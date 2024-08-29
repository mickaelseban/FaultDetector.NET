using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Locator;

namespace FaultDetectorDotNet.Tool
{
    public static class ProjectBuilder
    {
        public static string BuildProject(string projectFilePath)
        {
            MSBuildLocator.RegisterDefaults();
            return ProjectOutputPath(projectFilePath);
        }

        private static string ProjectOutputPath(string projectFilePath)
        {
            try
            {
                var projectCollection = new ProjectCollection();
                var buildParameters = new BuildParameters(projectCollection);

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