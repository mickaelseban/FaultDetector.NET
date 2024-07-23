using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaultDetectorDotNet.Core.Helpers
{
    public static class SanitizeHelper
    {
        public static string SanitizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            var invalidChars = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToArray();

            var invalidCharsPattern = $"[{Regex.Escape(new string(invalidChars))}]";

            var sanitizedPath = Regex.Replace(path, invalidCharsPattern, "_");

            return sanitizedPath;
        }
    }
}