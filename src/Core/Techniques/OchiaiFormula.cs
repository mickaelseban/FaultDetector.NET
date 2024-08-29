using System;

namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class OchiaiFormula : ISpectrumBasedFormula
    {
        public double Calculate(FormulaRequest request)
        {
            int NCF = request.Counts.failCount;
            int NF = request.TotalFailedTests; 
            int NCS = request.Counts.passCount;

            if (NCF == 0)
            {
                return 0.0;
            }

            if (NF == 0 || NCF + NCS == 0)
            {
                return 0;
            }

            return NCF / Math.Sqrt(NF * (NCF + NCS));
        }
    }
}