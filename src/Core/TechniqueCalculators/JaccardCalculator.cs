namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public sealed class JaccardCalculator : SpectrumBasedTechniqueCalculator
    {
        public override TechniqueType Type => TechniqueType.Jaccard;

        protected override double CalculateWithoutSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests)
        {
            var totalTests = counts.passCount + totalPassedTests + counts.failCount + totalFailedTests;

            if (totalTests == 0)
            {
                return 0;
            }

            return (double)(counts.failCount + totalFailedTests) / totalTests;
        }

        protected override double CalculateWithSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests,
            double symmetryCoefficient)
        {
            if (symmetryCoefficient == 0)
            {
                return 0;
            }

            var adjustedTotalPassed = counts.passCount * symmetryCoefficient + totalPassedTests;
            var totalTests = adjustedTotalPassed + counts.failCount + totalFailedTests;

            if (totalTests == 0)
            {
                return 0;
            }

            return counts.failCount / totalTests;
        }
    }
}