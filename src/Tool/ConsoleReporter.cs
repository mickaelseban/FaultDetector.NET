using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Tool
{
    public class ConsoleReporter : IReporter
    {
        private readonly ConsoleOutputLogger _processLogger;
        private readonly bool _splited;

        public ConsoleReporter(ConsoleOutputLogger processLogger, bool splited)
        {
            _processLogger = processLogger;
            _splited = splited;
        }

        public void Write(SuspiciousnessRunnerResult result)
        {
            ConsoleTablesResultWriter.Write(_processLogger.Output, result, _splited);
        }

        public bool CanWrite()
        {
            return true;
        }
    }
}