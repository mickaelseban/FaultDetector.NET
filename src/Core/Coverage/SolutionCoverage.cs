using System.Collections.Generic;

namespace FaultDetectorDotNet.Core.Coverage
{
    public sealed class SolutionCoverage
    {
        public Dictionary<string, Assembly> Assemblies { get; set; } = new Dictionary<string, Assembly>();


        public sealed class Assembly
        {
            public string Name { get; set; }
            public Dictionary<string, File> Files { get; set; } = new Dictionary<string, File>();
        }

        public sealed class File
        {
            public string SourcePath { get; set; }

            public Dictionary<string, Class> Classes { get; set; } = new Dictionary<string, Class>();
        }

        public sealed class Class
        {
            public string Name { get; set; }

            public Dictionary<string, Method> Methods { get; set; } = new Dictionary<string, Method>();
        }

        public sealed class Method
        {
            public string Signature { get; set; }
            public Dictionary<int, Line> Lines { get; set; } = new Dictionary<int, Line>();
        }

        public class Line
        {
            public int Number { get; set; }
            public int HitCount { get; set; }
        }
    }
}