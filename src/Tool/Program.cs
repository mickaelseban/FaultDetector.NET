using System;
using System.CommandLine;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;
using FaultDetectorDotNet.Tool.CustomArguments;

namespace FaultDetectorDotNet.Tool
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var verboseOption = new Option<bool>(
                new[] { "--verbose", "-v" },
                description: "Print Verbose Logs",
                getDefaultValue: () => false
            );

            var debugOption = new Option<bool>(
                new[] { "--debug", "-d" },
                description: "Enable debug mode",
                getDefaultValue: () => false
            );

            var splitSuspiciousnessResultsOption = new Option<bool>(
                new[] { "--slip-suspiciousness-results", "-ssr" },
                description: "Enable Split Suspiciousness Results. Default - Aggregated Results",
                getDefaultValue: () => false
            );

            var testProjectFullPathArgument = new TestProjectFullPathArgument();
            var techniquesOption = new TechniquesOption();
            var exportPathOption = new ExportPathOption();
            var rootCommand = new RootCommand
            {
                testProjectFullPathArgument,
                exportPathOption,
                verboseOption,
                debugOption,
                techniquesOption,
                splitSuspiciousnessResultsOption
            };

            rootCommand.SetHandler(async (testProjectFullPath,
                    exportPath,
                    verbose,
                    debugMode,
                    techniques,
                    splitSuspiciousnessResults) =>
                {
                    if (debugMode)
                    {
                        AttachDebugger();
                    }

                    var executionId = Guid.NewGuid().ToString();
                    using var processLogger = new ConsoleOutputLogger(verbose);
                    var parameters = new SuspiciousnessServiceParameters(executionId, 
                        testProjectFullPath,
                        techniques);
                    
                    var consoleReporter = new ConsoleReporter(processLogger, splitSuspiciousnessResults);
                    var textReporter = new TextReporter( executionId, exportPath, processLogger, splitSuspiciousnessResults);
                    var reportManager = new ReportManager(consoleReporter, textReporter);

                    await new SpectrumBasedFaultLocalizationRunner()
                    .Run(processLogger,
                        reportManager,
                        ProjectBuilder.BuildProject,
                        parameters,
                        CancellationToken.None);
                },
                testProjectFullPathArgument,
                exportPathOption,
                verboseOption,
                debugOption,
                techniquesOption,
                splitSuspiciousnessResultsOption);

            return await rootCommand.InvokeAsync(args);
        }

        private static void AttachDebugger()
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            Debugger.Break();
        }
    }
}