namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public sealed class DStarCalculator : SpectrumBasedTechniqueCalculator
    {
        public override TechniqueType Type => TechniqueType.DStar;

        // DStar = (c_ef^2) / (c_nf + c_ep)
        protected override double CalculateWithoutSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests)
        {
            double cef = counts.failCount;
            double cnf = totalFailedTests - counts.failCount;
            double cep = totalPassedTests;

            double denominator = cnf + cep;

            if (denominator == 0)
            {
                return 0;
            }

            var result = (cef * cef) / denominator;

            return result;
        }

        // DStar = (c_ef^2) / (c_nf * CS + c_ep)
        protected override double CalculateWithSymmetryCoefficient((int passCount, int failCount) counts, int totalPassedTests,
            int totalFailedTests, double symmetryCoefficient)
        {
            double cef = counts.failCount;
            double cnf = totalFailedTests - counts.failCount;
            double cep = totalPassedTests;

            double denominator = (cnf * symmetryCoefficient) + cep;

            if (denominator == 0)
            {
                return 0;
            }

            var result = (cef * cef) / denominator;

            return result;
        }
    }
}