using System;

namespace FaultDetectorDotNet.Core.Logger
{
    public interface IProcessLogger : IDisposable
    {
        void LogDebugMessage(string message);
        void LogDebugMessage(string message, Exception ex);
        void LogUserMessage(string message);
    }
}