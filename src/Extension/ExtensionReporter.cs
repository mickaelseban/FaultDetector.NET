using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Extension
{
    public sealed class ExtensionReporter : IReporter
    {
        private readonly DataGrid _coverageDataGrid;
        private readonly ObservableCollection<DataGridItem> _gridData;
        private readonly ObservableCollection<SuspiciousnessItem> _suspiciousnessResultItems;

        public ExtensionReporter(ObservableCollection<SuspiciousnessItem> suspiciousnessResultItems,
            ObservableCollection<DataGridItem> gridData,
            DataGrid coverageDataGrid)
        {
            _suspiciousnessResultItems = suspiciousnessResultItems;
            _gridData = gridData;
            _coverageDataGrid = coverageDataGrid;
        }

        public void Write(SuspiciousnessRunnerResult result)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LoadSuspiciousnessResult(_suspiciousnessResultItems, result.SuspiciousnessResult);
                LoadTestCoverageMatrix(_gridData, _coverageDataGrid, result.TestCoverageMatrix);
            });
        }

        public bool CanWrite()
        {
            return true;
        }

        private static List<SuspiciousnessItem> ConvertToSuspiciousnessResultItems(SuspiciousnessResult result)
        {
            var items = new List<SuspiciousnessItem>();

            foreach (var techniqueEntry in result.Techniques)
            {
                foreach (var assemblyEntry in techniqueEntry.Value.Assemblies)
                {
                    foreach (var fileEntry in assemblyEntry.Value.Files)
                    {
                        foreach (var classEntry in fileEntry.Value.Classes)
                        {
                            foreach (var methodEntry in classEntry.Value.Methods)
                            {
                                foreach (var lineEntry in methodEntry.Value.Lines)
                                {
                                    items.Add(new SuspiciousnessItem
                                    {
                                        Technique = techniqueEntry.Key.ToString(),
                                        Assembly = assemblyEntry.Key,
                                        File = fileEntry.Key,
                                        Class = classEntry.Key,
                                        Method = methodEntry.Key.Split(new[] { "::" }, StringSplitOptions.None).Last(),
                                        Line = lineEntry.Key,
                                        Score = lineEntry.Value.Score
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return items;
        }

        private static void LoadSuspiciousnessResult(ObservableCollection<SuspiciousnessItem> suspiciousnessResultItems,
            SuspiciousnessResult suspiciousnessResult)
        {
            suspiciousnessResultItems.Clear();

            if (suspiciousnessResult is null)
            {
                return;
            }

            foreach (var item in ConvertToSuspiciousnessResultItems(suspiciousnessResult))
            {
                suspiciousnessResultItems.Add(item);
            }
        }

        private static void LoadTestCoverageMatrix(ObservableCollection<DataGridItem> gridData,
            DataGrid coverageDataGrid, TestCoverageMatrix testCoverageMatrix)
        {
            gridData.Clear();

            if (testCoverageMatrix is null)
            {
                return;
            }

            var allTestNames = testCoverageMatrix.Matrix.Values
                .SelectMany(dict => dict.Keys)
                .Select(NormalizeTestName)
                .Distinct()
                .ToList();

            foreach (var lineId in testCoverageMatrix.Matrix.Keys)
            {
                var dataGridItem = new DataGridItem
                {
                    LineId = lineId.ToString(),
                    File = lineId.FilePath,
                    Class = lineId.ClassName,
                    Line = lineId.LineNumber,
                    Hits = new Dictionary<string, int>()
                };

                foreach (var originalTestName in testCoverageMatrix.Matrix[lineId].Keys)
                {
                    var normalizedTestName = NormalizeTestName(originalTestName);
                    dataGridItem.Hits[normalizedTestName] = testCoverageMatrix.Matrix[lineId][originalTestName];
                }

                gridData.Add(dataGridItem);
            }

            coverageDataGrid.Columns.Clear();
            coverageDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Line Id",
                Binding = new Binding("LineId"),
                ElementStyle = new Style(typeof(TextBlock))
                {
                    Setters =
                    {
                        new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(0, 0, 90)))
                    }
                }
            });
    
            for (var i = 0; i < allTestNames.Count; i++)
            {
                var testName = allTestNames[i];
                var originalTestName = testCoverageMatrix.Matrix.Values
                    .SelectMany(dict => dict.Keys)
                    .FirstOrDefault(name => NormalizeTestName(name) == testName);

                var testStatus = originalTestName != null ? testCoverageMatrix.TestStatus[originalTestName] : StatusType.Unknown;

                var columnHeader = new TextBlock
                {
                    Text = $"T{i + 1}",
                    ToolTip = $"Test: {testName} \n Status: {testStatus.ToString()}",
                    Width = 45,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                if (testStatus == StatusType.Failed)
                {
                    columnHeader.Background = Brushes.Red;
                }
                else if (testStatus == StatusType.Passed)
                {
                    columnHeader.Background = Brushes.LightGreen;
                }

                coverageDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = columnHeader,
                    Binding = new Binding($"Hits[{testName}]"),
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters =
                        {
                            new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(0, 0, 90)))
                        }
                    }
                });
            }

            coverageDataGrid.ItemsSource = gridData;
        }

        private static string NormalizeTestName(string testName)
        {
            return testName.Trim().Replace("(", "").Replace(")", "").Replace(",", "").Replace(" ", "_");
        }
    }
}