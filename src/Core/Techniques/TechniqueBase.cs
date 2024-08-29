using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Core.Techniques
{
    public abstract class TechniqueBase : ITechnique
    {
        public abstract ISpectrumBasedFormula Formula { get; }
        public abstract TechniqueType Type { get; }


        public SuspiciousnessResult.Technique Calculate(CoverageAggregatedModel coverageAggregated)
        {
            var result = new SuspiciousnessResult.Technique();

            DefaultCalculatorHelper.DefaultApplyCalulation(coverageAggregated, Formula, result);

            return result;
        }
    }
}