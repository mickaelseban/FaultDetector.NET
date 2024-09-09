using System.IO;
using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;
using Newtonsoft.Json;

namespace FaultDetectorDotNet.Tool
{
    public sealed class JsonReporter : IReporter
    {
        private readonly string _executionId;
        private readonly string _exportPath;
        private readonly IProcessLogger _processLogger;

        public JsonReporter(string executionId, string exportPath, IProcessLogger processLogger)
        {
            _executionId = executionId;
            _exportPath = exportPath;
            _processLogger = processLogger;
        }

        public void Write(SuspiciousnessRunnerResult result)
        {
            var filePath = Path.Combine(_exportPath, $"{_executionId}.json");

            _processLogger.LogUserMessage($"Exam Score exported a to {filePath}");

            var jsonString = JsonConvert.SerializeObject(result.SuspiciousnessResult, Formatting.Indented);

            using (var streamWriter = new StreamWriter(filePath))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
            }
        }

        public bool CanWrite()
        {
            return !(string.IsNullOrEmpty(_executionId) || string.IsNullOrEmpty(_exportPath));
        }
    }
}