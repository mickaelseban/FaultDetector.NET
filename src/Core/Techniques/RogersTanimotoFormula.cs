namespace FaultDetectorDotNet.Core.Techniques
{
    public class RogersTanimotoFormula : ISpectrumBasedFormula
    {
        public double Calculate(FormulaRequest request)
        {
            int NCF = request.Counts.failCount;
            int NCS = request.Counts.passCount; 
            int NUF = request.TotalFailedTests - NCF; 
            int NUP = request.TotalPassedTests - NCS; 

            if (NCF == 0)
            {
                return 0.0;
            }
            
            double numerator = NCF + NCS;
            double denominator = NCF + NCS + 2 * (NUF + NUP);

            if (denominator == 0)
            {
                return 0.0;
            }

            return numerator / denominator;
        }
    }
}