using System;

namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public class OchiaiCalculator : SpectrumBasedTechniqueCalculator
    {
        public override TechniqueType Type => TechniqueType.Ochiai;

        protected override double CalculateWithoutSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests)
        {
            if (totalFailedTests == 0 || counts.failCount + counts.passCount == 0)
            {
                return 0;
            }

            return counts.failCount / Math.Sqrt((counts.failCount + counts.passCount) * totalFailedTests);
        }

        protected override double CalculateWithSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests, double symmetryCoefficient)
        {
            if (symmetryCoefficient == 0)
            {
                return 0;
            }

            var adjustedPassCount = counts.passCount * symmetryCoefficient;
            if (totalFailedTests == 0 || counts.failCount + adjustedPassCount == 0)
            {
                return 0;
            }

            return counts.failCount / Math.Sqrt((counts.failCount + adjustedPassCount) * totalFailedTests);
        }
    }
}