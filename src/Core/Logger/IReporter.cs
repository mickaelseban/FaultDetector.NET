using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Core.Logger
{
    public interface IReporter
    {
        void Write(SuspiciousnessRunnerResult result);
    }
}