using System.Collections.Generic;
using FaultDetectorDotNet.Core.Coverlet;

namespace FaultDetectorDotNet.Core.Coverage
{
    public static class CoverageTestSegmentedFactory
    {
        public static Dictionary<string, SolutionCoverage> Create(
            Dictionary<string, CoverletCoverageData> data)
        {
            var segmentTestsCoverage = new Dictionary<string, SolutionCoverage>();

            foreach (var dataKv in data)
            {
                var solutionCoverage = new SolutionCoverage();
                segmentTestsCoverage[dataKv.Key] = solutionCoverage;

                foreach (var assemblyKv in dataKv.Value.Assembly)
                {
                    var assembly = new SolutionCoverage.Assembly();
                    solutionCoverage.Assemblies[assemblyKv.Key] = assembly;
                    assembly.Name = assemblyKv.Key;
                    foreach (var fileKv in assemblyKv.Value)
                    {
                        var file = new SolutionCoverage.File();
                        assembly.Files[fileKv.Key] = file;
                        file.SourcePath = fileKv.Key;

                        foreach (var classKv in fileKv.Value)
                        {
                            var @class = new SolutionCoverage.Class();
                            file.Classes[classKv.Key] = @class;
                            @class.Name = classKv.Key;

                            foreach (var methodKv in classKv.Value)
                            {
                                var method = new SolutionCoverage.Method();
                                @class.Methods[methodKv.Key] = method;
                                method.Signature = methodKv.Key;

                                foreach (var linesKv in methodKv.Value.Lines)
                                {
                                    var line = new SolutionCoverage.Line();
                                    method.Lines[linesKv.Key] = line;
                                    line.Number = linesKv.Key;
                                    line.HitCount = linesKv.Value;
                                }
                            }
                        }
                    }
                }
            }

            return segmentTestsCoverage;
        }
    }
}