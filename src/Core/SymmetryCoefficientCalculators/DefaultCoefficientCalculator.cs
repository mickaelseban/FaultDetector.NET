using FaultDetectorDotNet.Core.Coverage;

namespace FaultDetectorDotNet.Core.SymmetryCoefficientCalculators
{
    public class DefaultCoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Default;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return null;
        }
    }
}