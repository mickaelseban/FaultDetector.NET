using System.Collections.Generic;

namespace FaultDetectorDotNet.Core.Coverage
{
    public class CoverageAggregatedModel
    {
        public Dictionary<(string assemblyName, string fileName, string className, string methodName, int lineNumber), (int passCount, int failCount)> LineHitCounts
        { get; set; }

        public int TotalPassedTests { get; set; }
        public int TotalFailedTests { get; set; }
    }
}