using System.IO;
using System.Text.RegularExpressions;

namespace FaultDetectorDotNet.Core.Helpers
{
    public static class CodeReaderHelper
    {
        public static bool ShouldLineBeIgnored(string line)
        {
            var content = line.Trim();
            if (string.IsNullOrWhiteSpace(content))
            {
                return true;
            }

            if (content.StartsWith("{"))
            {
                return true;
            }

            if (content.EndsWith("}"))
            {
                return true;
            }

            if (Regex.IsMatch(content, @"\b(public|private|protected|internal)\b.*\b(class|interface|struct|enum)\b"))
            {
                return true;
            }

            return false;
        }

        public static string ReadLine(string filePath, int lineToRead)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            using (var streamReader = new StreamReader(filePath))
            {
                string line;
                var lineNumber = 0;

                while ((line = streamReader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (lineNumber == lineToRead)
                    {
                        return line;
                    }
                }
            }

            return null;
        }
    }
}