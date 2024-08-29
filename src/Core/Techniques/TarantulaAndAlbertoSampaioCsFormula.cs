namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class TarantulaAndAlbertoSampaioCsFormula : ISpectrumBasedFormula
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

            var failRate = (double)NCF / NF;
            var passRate = (double)NCS / NS;

            var denominator = failRate + CS * passRate;

            if (denominator == 0)
            {
                return 0.0;
            }

            return failRate / denominator;
        }
    }
}