using System.Linq;
using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.Helpers;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public static class SuspiciousnessCalculator
    {
        private static readonly ISpectrumBasedTechniqueCalculator[] TechniqueCalculatorStrategies =
        {
            new TarantulaCalculator(),
            new OchiaiCalculator(),
            new JaccardCalculator(),
            new DStarCalculator(),
            new KulczynskiCalculator(),
            new RogersTanimotoCalculator()
        };

        private static readonly ISymmetryCoefficientCalculator[] SymmetryCoefficientCalculator =
        {
            new DefaultCoefficientCalculator(),
            new Simplified01CoefficientCalculator(),
            new Simplified02CoefficientCalculator(),
            new Simplified03CoefficientCalculator(),
            new Simplified04CoefficientCalculator(),
            new Simplified05CoefficientCalculator(),
            new Simplified06CoefficientCalculator(),
            new Simplified07CoefficientCalculator(),
            new Simplified08CoefficientCalculator(),
            new Simplified09CoefficientCalculator(),
            new Simplified10CoefficientCalculator()
        };

        public static SuspiciousnessResult Calculate(CoverageAggregatedModel coverageAggregated, SuspiciousnessServiceParameters parameters)
        {
            var result = new SuspiciousnessResult();

            foreach (var requestTechnique in parameters.Techniques)
            {
                foreach (var asymmetryLevelType in parameters.SymmetryCoefficients)
                {
                    var technique = new SuspiciousnessResult.Technique();
                    result.Techniques[(requestTechnique, asymmetryLevelType)] = technique;

                    foreach (var item in coverageAggregated.LineHitCounts)
                    {
                        var key = item.Key;
                        var counts = item.Value;

                        (var assemblyName, var fileName, var className, var methodName, var lineNumber) = key;

                        var symmetryCoefficient = SymmetryCoefficientCalculator
                            .Single(x => x.Type == asymmetryLevelType)
                            .Calculate(counts, coverageAggregated);

                        var suspiciousness = TechniqueCalculatorStrategies
                            .Single(techniqueCalculator => techniqueCalculator.Type == requestTechnique)
                            .Calculate(counts, coverageAggregated.TotalPassedTests, coverageAggregated.TotalFailedTests,
                                symmetryCoefficient);

                        if (!result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies
                                .ContainsKey(assemblyName))
                        {
                            result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName] =
                                new SuspiciousnessResult.Assembly
                                    { Name = assemblyName };
                        }

                        if (!result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName].Files
                                .ContainsKey(fileName))
                        {
                            result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName]
                                    .Files[fileName] =
                                new SuspiciousnessResult.File
                                    { SourcePath = fileName };
                        }

                        if (!result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName]
                                .Files[fileName].Classes
                                .ContainsKey(className))
                        {
                            result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName]
                                    .Files[fileName]
                                    .Classes[className] =
                                new SuspiciousnessResult.Class { Name = className };
                        }

                        if (!result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName]
                                .Files[fileName]
                                .Classes[className]
                                .Methods
                                .ContainsKey(methodName))
                        {
                            result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName]
                                    .Files[fileName]
                                    .Classes[className]
                                    .Methods[methodName] =
                                new SuspiciousnessResult.Method { Signature = methodName };
                        }

                        var isLineIgnored =
                            CodeReaderHelper.ShouldLineBeIgnored(CodeReaderHelper.ReadLine(fileName, lineNumber));

                        if (!isLineIgnored)
                        {
                            result.Techniques[(requestTechnique, asymmetryLevelType)].Assemblies[assemblyName]
                                .Files[fileName]
                                .Classes[className]
                                .Methods[methodName]
                                .Lines[lineNumber] = new SuspiciousnessResult.Line
                            {
                                Number = lineNumber,
                                Score = suspiciousness
                            };
                        }
                    }
                }
            }

            return result;
        }
    }
}