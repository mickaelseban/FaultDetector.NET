using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.Coverlet;
using FaultDetectorDotNet.Core.Helpers;
using FaultDetectorDotNet.Core.Logger;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public sealed class SpectrumBasedFaultLocalizationRunner
    {
        public event EventHandler<SpectrumBasedFaultLocalizationRunnerStatusType> SpectrumBasedFaultLocalizationRunnerStatusChanged;

        public async Task Run(IProcessLogger logger, IReporter reporter, IProjectHelper projectHelper, SuspiciousnessServiceParameters inputParameters, CancellationToken cancellationToken)
        {
            SpectrumBasedFaultLocalizationRunnerStatusChanged?.Invoke(this, SpectrumBasedFaultLocalizationRunnerStatusType.Running);
            var testsMetadata = new Dictionary<string, TestMetadata>();
            try
            {
                logger.LogUserMessage("Process started");

                cancellationToken.ThrowIfCancellationRequested();

                if (await IsToolInstalled(logger, cancellationToken))
                {
                    logger.LogDebugMessage("The code coverage tool is already installed");
                }
                else
                {
                    logger.LogDebugMessage("Installing the code coverage tool...");
                    await InstallCoverageTool(logger, cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();

                var testProjectDllPath = projectHelper.BuildProject(inputParameters.TestProjectFullPath);

                if (string.IsNullOrEmpty(testProjectDllPath) || !File.Exists(testProjectDllPath))
                {
                    logger.LogUserMessage($"The project {inputParameters.TestProjectFullPath} was not found");
                    return;
                }

                testsMetadata = await ListTests(logger, inputParameters.TestProjectFullPath, cancellationToken);
                var tempPath = Path.Combine(Path.GetTempPath(), inputParameters.ExecutionId);
                Directory.CreateDirectory(tempPath);

                var coverletCoverageDatas = new Dictionary<string, CoverletCoverageData>();
                foreach (var test in testsMetadata)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await RunDotnetTestWithCoverage(logger, test.Value, testProjectDllPath, tempPath,
                        cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    var coverletCoverageData = CoverletService.ReadReport(test.Value.TestCoverageReportPath);
                    coverletCoverageDatas[test.Key] = coverletCoverageData;
                }

                cancellationToken.ThrowIfCancellationRequested();

                var coveragePerTest = CoverageTestSegmentedFactory.Create(coverletCoverageDatas);

                foreach (var test in testsMetadata)
                {
                    test.Value.Coverage = coveragePerTest[test.Key];
                }
                
                var coverageAggregated = CoverageFactory.CreateAggregated(testsMetadata);
                var testCoverageMatrix = CoverageFactory.CreateCoverageMatrix(testsMetadata);
                var suspiciousnessResult = SuspiciousnessCalculator.Calculate(coverageAggregated, inputParameters);
                var normalizatedSuspiciousnessResult = SuspiciousnessResultNormalizer.Normalize(coverageAggregated, inputParameters);
                
                cancellationToken.ThrowIfCancellationRequested();

                var suspiciousnessRunnerResult = new SuspiciousnessRunnerResult
                {
                    TestCoverageMatrix = testCoverageMatrix,
                    SuspiciousnessResult = suspiciousnessResult,
                    NormalizatedSuspiciousness = normalizatedSuspiciousnessResult
                };

                reporter.Write(suspiciousnessRunnerResult);
            }
            catch (OperationCanceledException)
            {
                logger.LogDebugMessage("The process was aborted from the user");
            }
            finally
            {
                logger.Dispose();
                DeleteTestCoverageFiles(logger, testsMetadata);
                SpectrumBasedFaultLocalizationRunnerStatusChanged?.Invoke(this, SpectrumBasedFaultLocalizationRunnerStatusType.Finished);
            }
        }

        private static void DeleteTestCoverageFiles(IProcessLogger logger, Dictionary<string, TestMetadata> testsMetadata)
        {
            foreach (var testMetadata in testsMetadata.Values)
            {
                try
                {
                    if (File.Exists(testMetadata.TestCoverageReportPath))
                    {
                        File.Delete(testMetadata.TestCoverageReportPath);
                        logger.LogDebugMessage($"File deleted: {testMetadata.TestCoverageReportPath}");
                    }
                }
                catch (IOException ioEx)
                {
                    logger.LogDebugMessage(
                        $"Could not delete the file: {testMetadata.TestCoverageReportPath}. Error: {ioEx.Message}");
                }
            }
        }

        private static async Task InstallCoverageTool(IProcessLogger logger, CancellationToken cancellationToken)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "tool install --global coverlet.console",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                await WaitForExit(logger, process, "coverlet.console installation", cancellationToken);

                if (process.ExitCode != 0)
                {
                    var errors = await process.StandardError.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        logger.LogDebugMessage(errors);
                    }
                }
            }
        }

        private static async Task WaitForExit(IProcessLogger logger, Process process, string processName, CancellationToken cancellationToken)
        {
            process.WaitForExit();
            while (!process.HasExited)
            {
                await Task.Delay(100, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    continue;
                }

                try
                {
                    process.Kill(); 
                }
                catch (Exception ex)
                {
                    logger.LogDebugMessage($"Error when trying to kill the process: {processName}", ex);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static async Task RunDotnetTestWithCoverage(IProcessLogger logger, TestMetadata testMetadata,
            string dllFilePath, string tempPath, CancellationToken cancellationToken)
        {
            var testCoveragePath = Path.Combine(tempPath, $"{SanitizeHelper.SanitizePath(testMetadata.TestQualifiedName)}.json");

            var testQualifiedNameSanitized = testMetadata.TestQualifiedName.Replace("(", "\\(").Replace(")", "\\)");
            var startInfo = new ProcessStartInfo
            {
                FileName = "coverlet",
                Arguments =
                    $"{dllFilePath} --target \"dotnet\" --targetargs \"vstest {dllFilePath} --TestCaseFilter:\\\"DisplayName~{testQualifiedNameSanitized}\\\"\" --output \"{testCoveragePath}\" --format json",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                var output = await process.StandardOutput.ReadToEndAsync();
                await WaitForExit(logger, process, "run coverlet", cancellationToken);
                logger.LogDebugMessage(output);
                
                if (process.ExitCode != 0)
                {
                    var errors = await process.StandardError.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(errors))
                    {
                        logger.LogDebugMessage(errors);
                    }
                }

                var status = OutputParserHelper.ExtractStatus(output);
                testMetadata.Status = status;
                testMetadata.TestCoverageReportPath = testCoveragePath;
                logger.LogDebugMessage($"Status for the test: {status}");
            }
        }

        private static async Task<bool> IsToolInstalled(IProcessLogger logger, CancellationToken cancellationToken)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "dotnet tool list -g",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                var output = await process.StandardOutput.ReadToEndAsync();
                await WaitForExit(logger, process, "verify coverlet installation", cancellationToken);
                return output.Contains("coverlet");
            }
        }

        private static async Task<Dictionary<string, TestMetadata>> ListTests(IProcessLogger logger, string TestProjectFullPath, CancellationToken cancellationToken)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"dotnet test {TestProjectFullPath} --list-tests",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                var output = process.StandardOutput.ReadToEnd();
                await WaitForExit(logger, process, "dotnet list tests", cancellationToken);

                var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var testNames = lines.Where(line => line.StartsWith("    ")).ToList();

                var testsMetadata = testNames.ToDictionary(x => x, testQualifiedName => new TestMetadata
                {
                    TestQualifiedName = testQualifiedName.Trim()
                });

                return testsMetadata;
            }
        }
    }
}