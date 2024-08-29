using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Core.Techniques
{
    public interface ITechnique
    {
        TechniqueType Type { get; }
        SuspiciousnessResult.Technique Calculate(CoverageAggregatedModel coverageAggregated);
    }
}