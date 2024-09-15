using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.Helpers;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Core.Techniques
{
    public static class DefaultCalculatorHelper
    {
        public static void DefaultApplyCalulation(CoverageAggregatedModel coverageAggregated,
            ISpectrumBasedFormula formula,
            SuspiciousnessResult.Technique resultTechnique)
        {
            foreach (var item in coverageAggregated.LineHitCounts)
            {
                var key = item.Key;
                var counts = item.Value;

                (var assemblyName, var fileName, var className, var methodName, var lineNumber) = key;

                var request = new FormulaRequest(counts,
                    coverageAggregated.TotalPassedTests,
                    coverageAggregated.TotalFailedTests);

                var suspiciousness = formula.Calculate(request);

                if (!resultTechnique.Assemblies.ContainsKey(assemblyName))
                {
                    resultTechnique.Assemblies[assemblyName] = new SuspiciousnessResult.Assembly
                        { Name = assemblyName };
                }

                if (!resultTechnique.Assemblies[assemblyName].Files
                        .ContainsKey(fileName))
                {
                    resultTechnique.Assemblies[assemblyName]
                            .Files[fileName] =
                        new SuspiciousnessResult.File
                            { SourcePath = fileName };
                }

                if (!resultTechnique.Assemblies[assemblyName]
                        .Files[fileName].Classes
                        .ContainsKey(className))
                {
                    resultTechnique.Assemblies[assemblyName]
                            .Files[fileName]
                            .Classes[className] =
                        new SuspiciousnessResult.Class { Name = className };
                }

                if (!resultTechnique.Assemblies[assemblyName]
                        .Files[fileName]
                        .Classes[className]
                        .Methods
                        .ContainsKey(methodName))
                {
                    resultTechnique.Assemblies[assemblyName]
                            .Files[fileName]
                            .Classes[className]
                            .Methods[methodName] =
                        new SuspiciousnessResult.Method { Signature = methodName };
                }

                var isLineIgnored = CodeReaderHelper.ShouldLineBeIgnored(fileName, lineNumber);

                if (!isLineIgnored)
                {
                    resultTechnique.Assemblies[assemblyName]
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
}