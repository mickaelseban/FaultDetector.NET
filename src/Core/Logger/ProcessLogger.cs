using System;
using System.Diagnostics;

namespace FaultDetectorDotNet.Core.Logger
{
    public abstract class ProcessLogger : IProcessLogger
    {
        private readonly Stopwatch _watch;

        protected ProcessLogger()
        {
            _watch = new Stopwatch();
            _watch.Start();
        }

        public void Dispose()
        {
            _watch.Stop();
            LogUserMessage($"Process completed in {Math.Round(_watch.Elapsed.TotalSeconds, 1)} seconds.");
        }

        public abstract void LogDebugMessage(string message);
        public abstract void LogDebugMessage(string message, Exception ex);
        public abstract void LogUserMessage(string message);
    }
}