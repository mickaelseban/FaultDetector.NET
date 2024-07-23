namespace FaultDetectorDotNet.Core.Coverage
{
    public class TestMetadata
    {
        public StatusType Status { get; set; } = StatusType.Unknown;
        public string TestQualifiedName { get; set; }
        public string TestCoverageReportPath { get; set; }
        public SolutionCoverage Coverage { get; set; }
    }
}