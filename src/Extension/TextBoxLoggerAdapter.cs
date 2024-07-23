using System;
using System.Reactive.Subjects;
using System.Windows.Controls;
using FaultDetectorDotNet.Core.Logger;

namespace FaultDetectorDotNet.Extension
{
    public class TextBoxLoggerAdapter : ProcessLogger
    {
        private readonly bool _verbose;
        private readonly Subject<string> _logSubject = new Subject<string>();

        public TextBoxLoggerAdapter(TextBox logTextBox, bool verbose = false)
        {
            _verbose = verbose;
            _logSubject.Subscribe(message =>
            {
                logTextBox.Dispatcher.Invoke(() =>
                {
                    logTextBox.AppendText($"{message}{Environment.NewLine}");
                    logTextBox.ScrollToEnd();
                });
            });
        }

        public override void LogDebugMessage(string message)
        {
            if (_verbose)
            {
                _logSubject.OnNext(message);
            }
        }

        public override void LogDebugMessage(string message, Exception ex)
        {
            if (_verbose)
            {
                _logSubject.OnNext(message + " - " + ex.Message);
            }
        }

        public override void LogUserMessage(string message)
        {
            _logSubject.OnNext(message);
        }
    }
}