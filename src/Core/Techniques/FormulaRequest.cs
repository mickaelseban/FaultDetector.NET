namespace FaultDetectorDotNet.Core.Techniques
{
    public class FormulaRequest
    {
        public FormulaRequest((int passCount, int failCount) counts, int totalPassedTests, int totalFailedTests)
        {
            Counts = counts;
            TotalPassedTests = totalPassedTests;
            TotalFailedTests = totalFailedTests;
        }

        public (int passCount, int failCount) Counts { get; }
        public int TotalPassedTests { get; }
        public int TotalFailedTests { get; }
    }
}