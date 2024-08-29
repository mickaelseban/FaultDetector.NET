using FaultDetectorDotNet.Core.Coverage;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public class SuspiciousnessRunnerResult
    {
        public SuspiciousnessResult SuspiciousnessResult { get; set; }
        public TestCoverageMatrix TestCoverageMatrix { get; set; }
    }
}