using System.IO;

namespace FaultDetectorDotNet.Core.Helpers
{
    public static class ProjectHelper
    {
        public static bool IsTestProject(string projectFilePath)
        {
            if (string.IsNullOrEmpty(projectFilePath))
            {
                return false;
            }

            try
            {
                if (File.Exists(projectFilePath))
                {
                    var projectFileContent = File.ReadAllText(projectFilePath);
                    if (projectFileContent.Contains("<PackageReference Include=\"Microsoft.NET.Test.Sdk\"") ||
                        projectFileContent.Contains("<PackageReference Include=\"MSTest.TestAdapter\"") ||
                        projectFileContent.Contains("<PackageReference Include=\"MSTest.TestFramework\"") ||
                        projectFileContent.Contains("<PackageReference Include=\"NUnit\"") ||
                        projectFileContent.Contains("<PackageReference Include=\"xunit\""))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}