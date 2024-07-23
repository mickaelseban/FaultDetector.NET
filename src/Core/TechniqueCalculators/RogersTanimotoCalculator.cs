namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public class RogersTanimotoCalculator : SpectrumBasedTechniqueCalculator
    {
        public override TechniqueType Type => TechniqueType.RogersTanimoto;

        protected override double CalculateWithoutSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests)
        {
            if (counts.failCount + 2 * (counts.passCount + (totalFailedTests - counts.failCount)) == 0)
            {
                return 0;
            }

            return (double)counts.failCount /
                   (counts.failCount + 2 * (counts.passCount + (totalFailedTests - counts.failCount)));
        }

        protected override double CalculateWithSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests, double symmetryCoefficient)
        {
            if (symmetryCoefficient == 0)
            {
                return 0;
            }

            var adjustedPassCount = counts.passCount * symmetryCoefficient;
            if (counts.failCount + 2 * (adjustedPassCount + (totalFailedTests - counts.failCount)) == 0)
            {
                return 0;
            }

            return counts.failCount /
                   (counts.failCount + 2 * (adjustedPassCount + (totalFailedTests - counts.failCount)));
        }
    }
}