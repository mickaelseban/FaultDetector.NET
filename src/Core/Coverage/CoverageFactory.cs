using System.Collections.Generic;
using System.Linq;
using FaultDetectorDotNet.Core.Helpers;

namespace FaultDetectorDotNet.Core.Coverage
{
    public class TestCoverageMatrix
    {
        public Dictionary<string, StatusType> TestStatus { get; } = new Dictionary<string, StatusType>();

        public Dictionary<LineId, Dictionary<string, int>> Matrix { get; } =
            new Dictionary<LineId, Dictionary<string, int>>();

        public void AddHit(string testName, StatusType status, LineId lineId, int hitCount)
        {
            if (!TestStatus.ContainsKey(testName))
            {
                TestStatus[testName] = status;
            }

            if (!Matrix.ContainsKey(lineId))
            {
                Matrix[lineId] = new Dictionary<string, int>();
            }

            if (!Matrix[lineId].ContainsKey(testName))
            {
                Matrix[lineId][testName] = 0;
            }

            Matrix[lineId][testName] += hitCount;
        }
    }

    public static class CoverageFactory
    {
        public static TestCoverageMatrix CreateCoverageMatrix(Dictionary<string, TestMetadata> coveragePerTest)
        {
            var testCoverageMatrix = new TestCoverageMatrix();

            foreach (var kvp in coveragePerTest)
            {
                var testName = kvp.Key.Trim();
                var testMetadata = kvp.Value;

                foreach (var assembly in testMetadata.Coverage.Assemblies.Values)
                {
                    foreach (var file in assembly.Files.Values)
                    {
                        foreach (var @class in file.Classes.Values)
                        {
                            foreach (var method in @class.Methods.Values)
                            {
                                foreach (var line in method.Lines.Values)
                                {
                                    var isLineIgnored = CodeReaderHelper.ShouldLineBeIgnored(CodeReaderHelper.ReadLine(file.SourcePath, line.Number));

                                    if (!isLineIgnored)
                                    {
                                        var lineId = new LineId(assembly.Name, @class.Name, file.SourcePath, line.Number);
                                        testCoverageMatrix.AddHit(testName, testMetadata.Status, lineId, line.HitCount);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return testCoverageMatrix;
        }


        public static CoverageAggregatedModel CreateAggregated(Dictionary<string, TestMetadata> coveragePerTest)
        {
            var coverageAggregated = new CoverageAggregatedModel
            {
                LineHitCounts =
                    new Dictionary<(string assemblyName, string fileName, string className, string methodName, int
                        lineNumber), (int passCount, int failCount)>(),
                TotalFailedTests = coveragePerTest.Values.Count(test => test.Status == StatusType.Failed),
                TotalPassedTests = coveragePerTest.Values.Count(test => test.Status == StatusType.Passed)
            };


            foreach (var testMetadata in coveragePerTest.Values)
            {
                foreach (var assembly in testMetadata.Coverage.Assemblies.Values)
                {
                    foreach (var file in assembly.Files.Values)
                    {
                        foreach (var @class in file.Classes.Values)
                        {
                            foreach (var method in @class.Methods.Values)
                            {
                                foreach (var line in method.Lines.Values)
                                {
                                    var key = (assembly.Name, file.SourcePath, @class.Name, method.Signature,
                                        line.Number);
                                    if (!coverageAggregated.LineHitCounts.ContainsKey(key))
                                    {
                                        coverageAggregated.LineHitCounts[key] = (0, 0);
                                    }

                                    if (line.HitCount > 0)
                                    {
                                        if (testMetadata.Status == StatusType.Passed)
                                        {
                                            coverageAggregated.LineHitCounts[key] = (
                                                coverageAggregated.LineHitCounts[key].passCount + 1,
                                                coverageAggregated.LineHitCounts[key].failCount);
                                        }
                                        else if (testMetadata.Status == StatusType.Failed)
                                        {
                                            coverageAggregated.LineHitCounts[key] = (
                                                coverageAggregated.LineHitCounts[key].passCount,
                                                coverageAggregated.LineHitCounts[key].failCount + 1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return coverageAggregated;
        }
    }
}