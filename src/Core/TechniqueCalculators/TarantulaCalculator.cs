namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public class TarantulaCalculator : SpectrumBasedTechniqueCalculator
    {
        public override TechniqueType Type => TechniqueType.Tarantula;

        protected override double CalculateWithoutSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests)
        {
            var passedRate = totalPassedTests == 0 ? 0 : (double)counts.passCount / totalPassedTests;
            var failedRate = totalFailedTests == 0 ? 0 : (double)counts.failCount / totalFailedTests;

            return passedRate + failedRate == 0 ? 0 : failedRate / (failedRate + passedRate);
        }

        protected override double CalculateWithSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests, double symmetryCoefficient)
        {
            if (symmetryCoefficient == 0)
            {
                return 0;
            }

            var adjustedTotalPassedTests = totalPassedTests * symmetryCoefficient;
            var passedRate = adjustedTotalPassedTests == 0 ? 0 : counts.passCount / adjustedTotalPassedTests;
            var failedRate = totalFailedTests == 0 ? 0 : (double)counts.failCount / totalFailedTests;

            return passedRate + failedRate == 0 ? 0 : failedRate / (failedRate + passedRate);
        }
    }
}