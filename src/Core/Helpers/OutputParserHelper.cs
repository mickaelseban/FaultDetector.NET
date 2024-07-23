using System;
using System.Text.RegularExpressions;
using FaultDetectorDotNet.Core.Coverage;

namespace FaultDetectorDotNet.Core.Helpers
{
    public static class OutputParserHelper
    {
        private static Regex TestStatusRegex { get; } =
            new Regex(@"\d+ test files matched the specified pattern.[\s\S]*?\n(Passed!|Failed!)", RegexOptions.IgnoreCase);


        public static StatusType ExtractStatus(string output)
        {
            var match = TestStatusRegex.Match(output);

            if (match.Success)
            {
                var value = match.Groups[1].Value;
                if (value.Equals("Passed!", StringComparison.OrdinalIgnoreCase))
                {
                    return StatusType.Passed;
                }

                if (value.Equals("Failed!", StringComparison.OrdinalIgnoreCase))
                {
                    return StatusType.Failed;
                }
            }

            return StatusType.Unknown;
        }
    }
}