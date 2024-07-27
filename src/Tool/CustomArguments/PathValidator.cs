using System;
using System.IO;

namespace FaultDetectorDotNet.Tool.CustomArguments
{
    public static class PathValidator
    {
        public static bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var invalidChars = Path.GetInvalidPathChars();
            if (path.IndexOfAny(invalidChars) >= 0)
            {
                return false;
            }

            try
            {
                var fullPath = Path.GetFullPath(path);
                return Uri.TryCreate(fullPath, UriKind.Absolute, out _) && fullPath.IndexOfAny(invalidChars) == -1;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}