using System;
using System.CommandLine;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Tool
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var testProjectFullPathArgument = new Argument<string>(
                "test-project-full-path",
                "The test project name"
            );

            var verboseOption = new Option<bool>(
                new[] { "--verbose", "-v" },
                description: "Print logs to the console (true or false)",
                getDefaultValue: () => false
            );

            var debugOption = new Option<bool>(
                new[] { "--debug", "-d" },
                description: "Enable or disable debug mode (true or false)",
                getDefaultValue: () => false
            );

            var exportPathOption = new Option<string>(
                new[] { "--exportPath", "-e" },
                "The path to export the results"
            );

            var spectrumBasedTechniquesOption = new Option<string[]>(
                new[] { "--techniques", "-t" },
                description: $"Spectrum-based techniques ({string.Join(" | ", Enum.GetNames<TechniqueType>())})",
                getDefaultValue: () => new[] { nameof(TechniqueType.Tarantula) }
            );

            var symmetryCoefficientsOption = new Option<string[]>(
                new[] { "--coefficients", "-c" },
                description: $"Symmetry Coefficient({string.Join(" | ", Enum.GetNames<SymmetryCoefficientType>())})",
                getDefaultValue: () => new[] {  nameof(SymmetryCoefficientType.Default) }
            );

            var allSpectrumBasedTechniquesOption = new Option<bool>(
                new[] { "--all-techniques", "-a" },
                description: "Enable all spectrum-based techniques",
                getDefaultValue: () => false
            );

            var rootCommand = new RootCommand
            {
                testProjectFullPathArgument,
                exportPathOption,
                verboseOption,
                debugOption,
                spectrumBasedTechniquesOption,
                allSpectrumBasedTechniquesOption,
                symmetryCoefficientsOption
            };

            rootCommand.SetHandler(async (testProjectFullPath,
                    exportPath,
                    verbose,
                    debugMode,
                    spectrumBasedTechnics,
                    allSpectrumBasedTechniques,
                    symmetryCoefficients) =>
                {
                    if (debugMode)
                    {
                        AttachDebugger();
                    }

                    var executionId = Guid.NewGuid().ToString();
                    using IProcessLogger processLogger = new ConsoleOutputLogger(verbose);
                    var parameters = SuspiciousnessServiceParametersFactory.Create(testProjectFullPath,
                        spectrumBasedTechnics,
                        allSpectrumBasedTechniques,
                        symmetryCoefficients,
                        executionId);
                    var reporter = new ToolReporter(executionId,
                        exportPath,
                        processLogger);
                    await new SpectrumBasedFaultLocalizationRunner().Run(processLogger,
                        reporter,
                        new ProjectHelper(),
                        parameters,
                        CancellationToken.None);

                },
                testProjectFullPathArgument,
                exportPathOption,
                verboseOption,
                debugOption,
                spectrumBasedTechniquesOption,
                allSpectrumBasedTechniquesOption,
                symmetryCoefficientsOption);

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