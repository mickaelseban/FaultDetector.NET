using System.Collections.Generic;
using FaultDetectorDotNet.Core.Techniques;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public sealed class SuspiciousnessResult
    {
        public Dictionary<TechniqueType, Technique> Techniques { get; set; } = new Dictionary<TechniqueType, Technique>();

        public sealed class Technique
        {
            public Dictionary<string, Assembly> Assemblies { get; set; } = new Dictionary<string, Assembly>();
        }

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
            public double Score { get; set; }
        }
    }
}