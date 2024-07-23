using FaultDetectorDotNet.Core.Coverage;

namespace FaultDetectorDotNet.Core.SymmetryCoefficientCalculators
{
    public class Simplified01CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified01;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.1;
        }
    }

    public class Simplified02CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified02;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.4;
        }
    }

    public class Simplified03CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified03;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.3;
        }
    }

    public class Simplified04CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified04;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.4;
        }
    }

    public class Simplified05CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified05;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.5;
        }
    }

    public class Simplified06CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified06;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.6;
        }
    }

    public class Simplified07CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified07;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.7;
        }
    }

    public class Simplified08CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified08;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.8;
        }
    }

    public class Simplified09CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified09;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 0.9;
        }
    }

    public class Simplified10CoefficientCalculator : ISymmetryCoefficientCalculator
    {
        public SymmetryCoefficientType Type => SymmetryCoefficientType.Simplified10;

        public double? Calculate((int passCount, int failCount) counts, CoverageAggregatedModel coverageAggregated)
        {
            return 1;
        }
    }
}