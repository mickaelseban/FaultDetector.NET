﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace FaultDetectorDotNet.Core.Helpers
{
    public static class ProjectHelper
    {
        public static (string Name, string Path) BuildProject(string projectFilePath)
        {
            try
            {
                var projectCollection = new ProjectCollection();
                var buildParameters = new BuildParameters(projectCollection)
                {
                    Loggers = new List<ILogger> { new ConsoleLogger(LoggerVerbosity.Normal) }
                };

                var buildRequest = new BuildRequestData(projectFilePath, new Dictionary<string, string>(), null, new[] { "Build" }, null);
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

        private static (string Name, string Path) GetProjectOutputPath(string projectFilePath)
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

                return (assemblyName, outputFilePath);
            }
            catch (Exception)
            {
                return default;
            }
        }

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
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}