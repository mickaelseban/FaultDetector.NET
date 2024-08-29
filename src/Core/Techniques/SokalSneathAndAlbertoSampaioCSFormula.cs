﻿namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class SokalSneathAndAlbertoSampaioCSFormula : ISpectrumBasedFormula
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
            double denominator = NF + 2 * (NCF + NCS) + NS + CS * (NCF + NCS);

            if (denominator == 0)
            {
                return 0.0;
            }

            return numerator / denominator;
        }
    }
}