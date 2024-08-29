namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class KulczynskiFormula : ISpectrumBasedFormula
    {
        public double Calculate(FormulaRequest request)
        {
            int NCF = request.Counts.failCount; 
            int NUF = request.TotalFailedTests - NCF;
            int NCS = request.Counts.passCount;

            if (NCF == 0)
            {
                return 0.0;
            }

            if (NCF + NUF == 0 || NCF + NCS == 0)
            {
                return 0.0;
            }

            double part1 = (double)NCF / (NCF + NUF);
            double part2 = (double)NCF / (NCF + NCS);

            return 0.5 * (part1 + part2);
        }
    }
}