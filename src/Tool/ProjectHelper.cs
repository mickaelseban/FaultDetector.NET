using System;
using System.Collections.Generic;
using System.IO;
using FaultDetectorDotNet.Core.Helpers;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;
using Microsoft.Build.Logging;

namespace FaultDetectorDotNet.Tool
{
    public class ProjectHelper : IProjectHelper
    {
        public string BuildProject(string projectFilePath)
        {
            MSBuildLocator.RegisterDefaults();
            return ProjectOutputPath(projectFilePath);
        }

        private static string ProjectOutputPath(string projectFilePath)
        {
            try
            {
                var projectCollection = new ProjectCollection();
                var buildParameters = new BuildParameters(projectCollection)
                {
                    Loggers = new List<ILogger> { new ConsoleLogger(LoggerVerbosity.Minimal) }
                };

                var buildRequest = new BuildRequestData(projectFilePath, new Dictionary<string, string>(), null,
                    new[] { "Build" }, null);
                var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

                if (buildResult.OverallResult == BuildResultCode.Success)
                {
                    return GetProjectOutputPath(projectFilePath);
                }

                throw new Exception("Build failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during build: {ex.Message}");
                return default;
            }
        }

        public bool IsTestProject(string projectFilePath)
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
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during test project check: {ex.Message}");
                return false;
            }

            return false;
        }

        private static string GetProjectOutputPath(string projectFilePath)
        {
            var projectCollection = new ProjectCollection();
            var project = projectCollection.LoadProject(projectFilePath);
            try
            {
                var outputPath = project.GetPropertyValue("OutputPath");
                var projectDirectory = Path.GetDirectoryName(projectFilePath);
                var fullOutputPath = Path.Combine(projectDirectory, outputPath);
                var assemblyName = project.GetPropertyValue("AssemblyName");
                var outputFilePath = Path.Combine(fullOutputPath, assemblyName + ".dll");

                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during path retrieval: {ex.Message}");
                return default;
            }
        }
    }
}