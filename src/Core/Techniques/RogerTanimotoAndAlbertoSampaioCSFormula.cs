namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class RogerTanimotoAndAlbertoSampaioCSFormula : ISpectrumBasedFormula
    {
        public double Calculate(FormulaRequest request)
        {
            var NCF = request.Counts.failCount;
            var NF = request.TotalFailedTests;
            var NCS = request.Counts.passCount;
            var NS = request.TotalPassedTests;

            if (NCF == 0)
            {
                return 0.0;
            }

            var CS = AlbertoSampaioSymmetryCoefficient.Calculate(NCF, NCS);

            double numerator = 2 * (NCF + NCS);
            var denominator = 2 * (NF + NS) + (1 + CS) * (NCF + NCS);

            if (denominator == 0)
            {
                return 0.0;
            }

            return numerator / denominator;
        }
    }
}