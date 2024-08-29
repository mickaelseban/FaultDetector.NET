namespace FaultDetectorDotNet.Core.Techniques
{
    public static class AlbertoSampaioSymmetryCoefficient
    {
        public static double Calculate(int NCF, int NCS)
        {
            if (NCF + NCS == 0)
            {
                return 1.0;
            }

            return (double)NCS / (NCS + NCF);
        }
    }
}