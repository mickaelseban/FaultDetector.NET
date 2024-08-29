namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class TarantulaFormula : ISpectrumBasedFormula
    {
        public double Calculate(FormulaRequest request)
        {
            var NCF = request.Counts.failCount; 
            var NCS = request.Counts.passCount; 
            var NF = request.TotalFailedTests; 
            var NS = request.TotalPassedTests; 

            if (NCF == 0)
            {
                return 0.0;
            }

            if (NF == 0 || NS == 0 || NCS == 0)
            {
                return 0.0;
            }

            var numerator = (double)NCF / NF;
            var denominator = numerator + (double)NCS / NS;

            if (denominator == 0)
            {
                return 0.0;
            }

            var suspiciousness = numerator / denominator;
            return suspiciousness;
        }
    }
}