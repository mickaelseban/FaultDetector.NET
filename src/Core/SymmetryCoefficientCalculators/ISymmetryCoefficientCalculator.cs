using FaultDetectorDotNet.Core.Coverage;

namespace FaultDetectorDotNet.Core.SymmetryCoefficientCalculators
{
    public interface ISymmetryCoefficientCalculator
    {
        SymmetryCoefficientType Type { get; }

        double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated);
    }
}