namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public class KulczynskiCalculator : SpectrumBasedTechniqueCalculator
    {
        public override TechniqueType Type => TechniqueType.Kulczynski;

        protected override double CalculateWithoutSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests)
        {
            if (counts.failCount + (totalPassedTests - counts.passCount) == 0 ||
                counts.passCount + counts.failCount == 0)
            {
                return 0;
            }

            var term1 = counts.failCount / (double)(counts.failCount + (totalPassedTests - counts.passCount));
            var term2 = counts.failCount / (double)(counts.passCount + counts.failCount);
            return 0.5 * (term1 + term2);
        }

        protected override double CalculateWithSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests, double symmetryCoefficient)
        {
            if (symmetryCoefficient == 0)
            {
                return 0;
            }

            if (counts.failCount + symmetryCoefficient * (totalPassedTests - counts.passCount) == 0 ||
                symmetryCoefficient * counts.passCount + counts.failCount == 0)
            {
                return 0;
            }

            var term1 = counts.failCount /
                        (counts.failCount + symmetryCoefficient * (totalPassedTests - counts.passCount));
            var term2 = counts.failCount / (symmetryCoefficient * counts.passCount + counts.failCount);
            return 0.5 * (term1 + term2);
        }
    }
}