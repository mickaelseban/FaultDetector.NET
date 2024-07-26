using System;
using System.IO;

namespace FaultDetectorDotNet.Core.Logger
{
    public interface IProcessLogger : IDisposable
    {
        TextWriter Output { get; }
        void LogDebugMessage(string message);
        void LogDebugMessage(string message, Exception ex);
        void LogUserMessage(string message);
    }
}