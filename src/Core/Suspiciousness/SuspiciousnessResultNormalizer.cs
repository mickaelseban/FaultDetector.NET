using System;
using System.Linq;
using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public static class SuspiciousnessResultNormalizer
    {
        public static NormalizatedSuspiciousness Normalize(CoverageAggregatedModel coverageAggregated, SuspiciousnessServiceParameters parameters)
        {
            if (!parameters.NormalizatedTechnique)
            {
                return null;
            }

            var suspiciousnessResult = SuspiciousnessCalculator.Calculate(coverageAggregated, SuspiciousnessServiceParametersFromNormalizationFactory.Create(parameters));

            var result = new NormalizatedSuspiciousness();
            MapFromSuspiciousnessResult(suspiciousnessResult, result);
            NormalizeScores(result);
            FilterOutliers(result);

            return result;
        }

        private static void RemoveLowScoreOutliers(NormalizatedSuspiciousness result, double calculatedPercentil)
        {
            foreach (var assembly in result.Assemblies.Values.ToList())
            {
                foreach (var file in assembly.Files.Values.ToList())
                {
                    foreach (var clazz in file.Classes.Values.ToList())
                    {
                        foreach (var method in clazz.Methods.Values.ToList())
                        {
                            var linesToRemove = method.Lines.Values.Where(line => line.Score < calculatedPercentil)
                                .Select(line => line.Number).ToList();

                            foreach (var lineNumber in linesToRemove)
                            {
                                method.Lines.Remove(lineNumber);
                            }

                            if (method.Lines.Count == 0)
                            {
                                clazz.Methods.Remove(method.Signature);
                            }
                        }

                        if (clazz.Methods.Count == 0)
                        {
                            file.Classes.Remove(clazz.Name);
                        }
                    }

                    if (file.Classes.Count == 0)
                    {
                        assembly.Files.Remove(file.SourcePath);
                    }
                }

                if (assembly.Files.Count == 0)
                {
                    result.Assemblies.Remove(assembly.Name);
                }
            }
        }

        private static void NormalizeScores(NormalizatedSuspiciousness result)
        {
            var maxScore = result.Assemblies.Values
                .SelectMany(a => a.Files
                    .SelectMany(f => f.Value.Classes.Values
                        .SelectMany(c => c.Methods.Values
                            .SelectMany(m => m.Lines.Values
                                .Select(l => l.Score)))))
                .Max();


            foreach (var assembly in result.Assemblies.Values)
            {
                foreach (var file in assembly.Files.Values)
                {
                    foreach (var clazz in file.Classes.Values)
                    {
                        foreach (var method in clazz.Methods.Values)
                        {
                            foreach (var lineNumber in method.Lines)
                            {
                                lineNumber.Value.Score = Math.Round(lineNumber.Value.Score / maxScore, 2);
                            }
                        }
                    }
                }
            }
        }

        private static void FilterOutliers(NormalizatedSuspiciousness result)
        {
            var linesScore = result.Assemblies.Values
                .SelectMany(a => a.Files
                    .SelectMany(f => f.Value.Classes.Values
                        .SelectMany(c => c.Methods.Values
                            .SelectMany(m => m.Lines.Values
                                .Select(l => l.Score)))))
                .ToArray();

            var percentileThreshold = CalculatePercentile(linesScore, 90);
            RemoveLowScoreOutliers(result, percentileThreshold);
        }

        private static double CalculatePercentile(double[] linesScore, double percentil)
        {
            Array.Sort(linesScore);
            var k = percentil / 100.0 * (linesScore.Length - 1);
            var f = (int)Math.Floor(k);
            var c = (int)Math.Ceiling(k);
            if (f == c)
            {
                return linesScore[f];
            }

            return linesScore[f] + (k - f) * (linesScore[c] - linesScore[f]);
        }


        private static void MapFromSuspiciousnessResult(SuspiciousnessResult suspiciousnessResult,
            NormalizatedSuspiciousness result)
        {
            foreach (var techniqueKv in suspiciousnessResult.Techniques)
            {
                foreach (var assemblyKv in techniqueKv.Value.Assemblies)
                {
                    if (techniqueKv.Key.Item2 != SymmetryCoefficientType.Default)
                    {
                        continue;
                    }

                    if (!result.Assemblies.TryGetValue(assemblyKv.Key, out var resultAssembly))
                    {
                        resultAssembly = new NormalizatedSuspiciousness.Assembly();
                        result.Assemblies[assemblyKv.Key] = resultAssembly;
                    }

                    resultAssembly.Name = assemblyKv.Key;

                    foreach (var fileKv in assemblyKv.Value.Files)
                    {
                        if (!resultAssembly.Files.TryGetValue(fileKv.Key, out var resultFile))
                        {
                            resultFile = new NormalizatedSuspiciousness.File();
                            resultAssembly.Files[fileKv.Key] = resultFile;
                        }

                        resultFile.SourcePath = fileKv.Key;

                        foreach (var classKv in fileKv.Value.Classes)
                        {
                            if (!resultFile.Classes.TryGetValue(classKv.Key, out var resultClass))
                            {
                                resultClass = new NormalizatedSuspiciousness.Class();
                                resultFile.Classes[classKv.Key] = resultClass;
                            }

                            resultClass.Name = classKv.Key;

                            foreach (var methodKv in classKv.Value.Methods)
                            {
                                if (!resultClass.Methods.TryGetValue(methodKv.Key, out var resultMethod))
                                {
                                    resultMethod = new NormalizatedSuspiciousness.Method();
                                    resultClass.Methods[methodKv.Key] = resultMethod;
                                }

                                resultMethod.Signature = methodKv.Key;

                                foreach (var linesKv in methodKv.Value.Lines)
                                {
                                    if (!resultMethod.Lines.TryGetValue(linesKv.Key, out var resultLine))
                                    {
                                        resultLine = new NormalizatedSuspiciousness.Line();
                                        resultMethod.Lines[linesKv.Key] = resultLine;
                                    }

                                    resultLine.Number = linesKv.Key;
                                    resultLine.Score += linesKv.Value.Score;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}