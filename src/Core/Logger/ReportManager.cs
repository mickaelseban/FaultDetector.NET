using System.Linq;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Core.Logger
{
    public sealed class ReportManager
    {
        private readonly IReporter[] _reporters;

        public ReportManager(params IReporter[] reporters)
        {
            _reporters = reporters.ToArray();
        }

        public void ReportAll(SuspiciousnessRunnerResult result)
        {
            foreach (var reporter in _reporters)
            {
                if (reporter.CanWrite())
                {
                    reporter.Write(result);
                }
            }
        }
    }
}