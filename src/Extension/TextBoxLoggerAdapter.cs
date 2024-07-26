using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using FaultDetectorDotNet.Core.Logger;

namespace FaultDetectorDotNet.Extension
{
    public class TextBoxLoggerAdapter : ProcessLogger
    {
        private readonly bool _verbose;

        public TextBoxLoggerAdapter(TextBox logTextBox, bool verbose = false)
        {
            _verbose = verbose;
            Output = new TextBoxWriter(logTextBox);
        }

        public override TextWriter Output { get; }

        public override void LogDebugMessage(string message)
        {
            if (_verbose)
            {
                LogMessage(message);
            }
        }

        public override void LogDebugMessage(string message, Exception ex)
        {
            if (_verbose)
            {
                LogMessage($"{message} - {ex.Message}");
            }
        }

        public override void LogUserMessage(string message)
        {
            LogMessage(message);
        }

        private void LogMessage(string message)
        {
            Output.WriteLine(message);
        }

        public sealed class TextBoxWriter : TextWriter
        {
            private readonly TextBox _logTextBox;

            public TextBoxWriter(TextBox logTextBox)
            {
                _logTextBox = logTextBox;
            }

            public override Encoding Encoding => Encoding.UTF8;

            public override void WriteLine(string value)
            {
                _logTextBox.Dispatcher.Invoke(() =>
                {
                    _logTextBox.AppendText($"{value}{Environment.NewLine}");
                    _logTextBox.ScrollToEnd();
                });
            }

            public override void Write(char value)
            {
                Write(value.ToString());
            }

            public override void Write(string value)
            {
                _logTextBox.Dispatcher.Invoke(() =>
                {
                    _logTextBox.AppendText(value);
                    _logTextBox.ScrollToEnd();
                });
            }
        }
    }
}