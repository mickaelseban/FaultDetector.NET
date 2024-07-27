using System.Collections.Generic;
using System.IO;
using ConsoleTables;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Tool
{
    public static class ConsoleTablesResultWriter
    {
        public static void Write(TextWriter textWriter, SuspiciousnessRunnerResult result, bool splited)
        {
            if (splited)
            {
                PrintSuspiciousnessSplited(textWriter, result.SuspiciousnessResult);
            }
            else
            {
                PrintSuspiciousness(textWriter, result.SuspiciousnessResult);
            }
            PrintNormalizatedSuspiciousness(textWriter, result.NormalizatedSuspiciousness);
        }

        private static void PrintSuspiciousness(TextWriter textWriter, SuspiciousnessResult result)
        {
            if (result == null)
            {
                return;
            }

            var options = new ConsoleTableOptions
            {
                Columns = new List<string> { "Technique, Coefficient", "Class", "Method", "Line", "Score" },
                OutputTo = textWriter,
                EnableCount = false
            };

            var table = SuspiciousnessTable(result);

            textWriter.WriteLine("# Suspiciousness ####################");
            var consoleTable = new ConsoleTable(options);

            foreach (var row in table)
            {
                consoleTable.AddRow(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5);
            }

            consoleTable.Write();
        }

        private static void PrintSuspiciousnessSplited(TextWriter textWriter, SuspiciousnessResult result)
        {
            if (result == null)
            {
                return;
            }

            var options = new ConsoleTableOptions
            {
                Columns = new List<string> { "Class", "Method", "Line", "Score" },
                OutputTo = textWriter,
                EnableCount = false
            };

            var tablesInfo = SuspiciousnessTableInfo(result);

            foreach (var tableInfo in tablesInfo)
            {
                textWriter.WriteLine($"# Suspiciousness - {tableInfo.Name} ####################");
                var consoleTable = new ConsoleTable(options);

                foreach (var row in tableInfo.Table)
                {
                    consoleTable.AddRow(row.Item1, row.Item2, row.Item3, row.Item4);
                }

                consoleTable.Write();
            }
        }

        private static List<(string Name, List<(string, string, int, double)> Table)> SuspiciousnessTableInfo(
            SuspiciousnessResult result)
        {
            var tablesInfo = new List<(string, List<(string,string, int, double)>)>();

            foreach (var technique in result.Techniques)
            {
                var table = new List<(string, string, int, double)>();
                tablesInfo.Add((technique.Key.ToString(), table));

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
                                        table.Add((@class.Name, method.Signature, line.Number,
                                            line.Score));
                                    }
                                }
                            }
                        }
                    }
                }

                table.Sort((x, y) => y.Item4.CompareTo(x.Item4));
            }

            return tablesInfo;
        }

        private static List<(string, string, string, int, double)> SuspiciousnessTable(SuspiciousnessResult result)
        {
            var table = new List<(string, string, string, int, double)>();
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
                                        table.Add((technique.Key.ToString(), @class.Name, method.Signature, line.Number,
                                            line.Score));
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

            textWriter.WriteLine("# Normalizated Suspiciousness ####################");

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
        }

        private static List<(string, string, int, double)> PrintNormalizatedSuspiciousnessTable(
            NormalizatedSuspiciousness result)
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