using System.IO;
using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;

namespace FaultDetectorDotNet.Tool
{
    public sealed class TextReporter : IReporter
    {
        private readonly string _executionId;
        private readonly string _exportPath;
        private readonly IProcessLogger _processLogger;
        private readonly bool _splited;

        public TextReporter(string executionId, string exportPath, IProcessLogger processLogger, bool splited)
        {
            _executionId = executionId;
            _exportPath = exportPath;
            _processLogger = processLogger;
            _splited = splited;
        }

        public void Write(SuspiciousnessRunnerResult result)
        {
            var filePath = Path.Combine(_exportPath, $"{_executionId}.txt");

            _processLogger.LogUserMessage($"Result exported as Text to {filePath}");

            using (var streamWriter = new StreamWriter(filePath))
            {
                ConsoleTablesResultWriter.Write(streamWriter, result, _splited);
                streamWriter.Flush();
            }
        }

        public bool CanWrite()
        {
            return !(string.IsNullOrEmpty(_executionId) || string.IsNullOrEmpty(_exportPath));
        }
    }
}

