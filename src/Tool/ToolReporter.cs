using System.IO;
using FaultDetectorDotNet.Core.Logger;
using FaultDetectorDotNet.Core.Suspiciousness;
using Newtonsoft.Json;

namespace FaultDetectorDotNet.Tool
{
    public class ToolReporter : IReporter
    {
        private readonly string _executionId;
        private readonly string _exportPath;
        private readonly IProcessLogger _processLogger;

        public ToolReporter(string executionId, string exportPath, IProcessLogger processLogger)
        {
            _executionId = executionId;
            _exportPath = exportPath;
            _processLogger = processLogger;
        }

        public void Write(SuspiciousnessRunnerResult result)
        {
            var filePath = Path.Combine(_exportPath, $"{_executionId}.json");
            var serializeObject = JsonConvert.SerializeObject(result, Formatting.Indented);
            File.WriteAllText(filePath, serializeObject);
            _processLogger.LogUserMessage($"Process finished and exported to {filePath}");
            PrintResult(_processLogger, result);
        }

        private static void PrintResult(IProcessLogger logger, SuspiciousnessRunnerResult result)
        {
            logger.LogUserMessage("################# Suspiciousness ####################");
            
            foreach (var technique in result.SuspiciousnessResult.Techniques)
            {
                foreach (var assembly in technique.Value.Assemblies.Values)
                {
                    foreach (var file in assembly.Files.Values)
                    {
                        foreach (var @class in file.Classes.Values)
                        {
                            foreach (var method in @class.Methods.Values)
                            {
                                foreach (var line in method.Lines.Values)
                                {
                                    logger.LogUserMessage($"Assembly: {assembly.Name}, File: {file.SourcePath}, Class: {@class.Name}, Method: {method.Signature}, Line: {line.Number}, Score: {line.Score}");
                                }
                            }
                        }
                    }
                }
            }

            logger.LogUserMessage("#####################################");
        }
    }
}