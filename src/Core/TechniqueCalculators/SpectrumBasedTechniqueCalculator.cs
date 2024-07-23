namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public abstract class SpectrumBasedTechniqueCalculator : ISpectrumBasedTechniqueCalculator
    {
        public abstract TechniqueType Type { get; }

        public double Calculate((int passCount, int failCount) counts, int totalPassedTests, int totalFailedTests,
            double? symmetryCoefficient)
        {
            if (symmetryCoefficient.HasValue)
            {
                return CalculateWithSymmetryCoefficient(counts, totalPassedTests, totalFailedTests, symmetryCoefficient.GetValueOrDefault());
            }

            return CalculateWithoutSymmetryCoefficient(counts, totalPassedTests, totalFailedTests);
        }

        protected abstract double CalculateWithoutSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests, int totalFailedTests);

        protected abstract double CalculateWithSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests, int totalFailedTests, double symmetryCoefficient);
    }
}