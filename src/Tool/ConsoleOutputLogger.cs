using System;
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

        public override void LogDebugMessage(string message)
        {
            if (_verboseMode)
            {
                Console.WriteLine(message);
            }
        }

        public override void LogDebugMessage(string message, Exception ex)
        {
            if (_verboseMode)
            {
                Console.WriteLine(message + " - " + ex.Message);
            }
        }

        public override void LogUserMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}