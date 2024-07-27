using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace FaultDetectorDotNet.Extension
{
    public static class ProjectBuilder
    {
        public static string BuildProject(string projectFilePath)
        {
            try
            {
                var projectCollection = new ProjectCollection();
                var buildParameters = new BuildParameters(projectCollection)
                {
                    Loggers = new List<ILogger> { new ConsoleLogger(LoggerVerbosity.Quiet) }
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
            catch (Exception)
            {
                return default;
            }
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
            catch (Exception)
            {
                return default;
            }
        }
    }
}