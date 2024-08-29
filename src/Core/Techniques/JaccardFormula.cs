namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class JaccardFormula : ISpectrumBasedFormula
    {
        public double Calculate(FormulaRequest request)
        {
            int NCF = request.Counts.failCount;
            int NUF = request.TotalFailedTests - request.Counts.failCount;
            int NCS = request.Counts.passCount;

            if (NCF == 0)
            {
                return 0.0;
            }

            int denominator = NCF + NUF + NCS;

            if (denominator == 0)
            {
                return 0;
            }

            return (double)NCF / denominator;
        }
    }
}