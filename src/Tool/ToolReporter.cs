using System.Collections.Generic;
using System.IO;
using ConsoleTables;
using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Tool
{
    public class ToolReporter : IReporter
    {
        private readonly string _executionId;
        private readonly string _exportPath;
        private readonly IProcessLogger _processLogger;

        public ToolReporter(string executionId, string exportPath, IProcessLogger processLogger)
        {
            _executionId = executionId;
            _exportPath = exportPath;
            _processLogger = processLogger;
        }

        public void Write(SuspiciousnessRunnerResult result)
        {
            var filePath = Path.Combine(_exportPath, $"{_executionId}.txt");

            _processLogger.LogUserMessage($"Process finished and exported to {filePath}");

            using (var streamWriter = new StreamWriter(filePath))
            {
                WriteResult(streamWriter, result);
                streamWriter.Flush();
            }

            WriteResult(_processLogger.Output, result);
        }

        private static void WriteResult(TextWriter textWriter, SuspiciousnessRunnerResult result)
        {
            PrintSuspiciousness(textWriter, result.SuspiciousnessResult);
            PrintNormalizatedSuspiciousness(textWriter, result.NormalizatedSuspiciousness);
        }

        private static void PrintSuspiciousness(TextWriter textWriter, SuspiciousnessResult result)
        {
            textWriter.WriteLine("################# Suspiciousness ####################");

            var options = new ConsoleTableOptions
            {
                Columns = new List<string> { "Class", "Method", "Line", "Score" },
                OutputTo = textWriter,
                EnableCount = false
            };

            var consoleTable = new ConsoleTable(options);

            foreach (var row in SuspiciousnessTable(result))
            {
                consoleTable.AddRow(row.Item1, row.Item2, row.Item3, row.Item4);
            }

            consoleTable.Write();

            textWriter.WriteLine("#####################################");
        }

        private static List<(string, string, int, double)> SuspiciousnessTable(SuspiciousnessResult result)
        {
            var table = new List<(string, string, int, double)>();
            foreach (var technique in result.Techniques)
            {
                foreach (var assembly in technique.Value.Assemblies.Values)
                {
                    foreach (var file in assembly.Files.Values)
                    {
                        foreach (var @class in file.Classes.Values)
                        {
                            foreach (var method in @class.Methods.Values)
                            {
                                foreach (var line in method.Lines.Values)
                                {
                                    if (line.Score > 0)
                                    {
                                        table.Add((@class.Name, method.Signature, line.Number, line.Score));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            table.Sort((x, y) => y.Item4.CompareTo(x.Item4));
            return table;
        }

        private static void PrintNormalizatedSuspiciousness(TextWriter textWriter, NormalizatedSuspiciousness result)
        {
            if (result == null)
            {
                return;
            }

            textWriter.WriteLine("################# Normalizated Suspiciousness ####################");

            var options = new ConsoleTableOptions
            {
                Columns = new List<string> { "Class", "Method", "Line", "Score" },
                OutputTo = textWriter,
                EnableCount = false
            };

            var consoleTable = new ConsoleTable(options);

            foreach (var row in PrintNormalizatedSuspiciousnessTable(result))
            {
                consoleTable.AddRow(row.Item1, row.Item2, row.Item3, row.Item4);
            }

            consoleTable.Write();
            textWriter.WriteLine("#####################################");
        }

        private static List<(string, string, int, double)> PrintNormalizatedSuspiciousnessTable(NormalizatedSuspiciousness result)
        {
            var table = new List<(string, string, int, double)>();

            foreach (var assembly in result.Assemblies.Values)
            {
                foreach (var file in assembly.Files.Values)
                {
                    foreach (var @class in file.Classes.Values)
                    {
                        foreach (var method in @class.Methods.Values)
                        {
                            foreach (var line in method.Lines.Values)
                            {
                                if (line.Score > 0)
                                {
                                    table.Add((@class.Name, method.Signature, line.Number, line.Score));
                                }
                            }
                        }
                    }
                }
            }


            table.Sort((x, y) => y.Item4.CompareTo(x.Item4));
            return table;
        }
    }
}