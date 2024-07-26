using System;
using System.IO;
using FaultDetectorDotNet.Core.Logger;

namespace FaultDetectorDotNet.Tool
{
    public class ConsoleOutputLogger : ProcessLogger
    {
        private readonly bool _verboseMode;

        public ConsoleOutputLogger(bool verboseMode)
        {
            _verboseMode = verboseMode;
        }

        public override TextWriter Output { get; } = Console.Out;

        public override void LogDebugMessage(string message)
        {
            if (_verboseMode)
            {
                Output.WriteLine(message);
            }
        }

        public override void LogDebugMessage(string message, Exception ex)
        {
            if (_verboseMode)
            {
                Output.WriteLine(message + " - " + ex.Message);
            }
        }

        public override void LogUserMessage(string message)
        {
            Output.WriteLine(message);
        }
    }
}