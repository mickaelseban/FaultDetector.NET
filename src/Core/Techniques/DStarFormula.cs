using System;

namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class DStarFormula : ISpectrumBasedFormula
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
            double denominator = NCS + NUF;

            if (denominator == 0)
            {
                return 0.0;
            }

            return Math.Pow(NCF, 2) / denominator;
        }
    }
}