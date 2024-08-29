using System.Collections.Generic;
using System.Linq;
using FaultDetectorDotNet.Core.Techniques;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public class SuspiciousnessServiceParameters
    {
        public SuspiciousnessServiceParameters(string executionId,
            string testProjectFullPath,
            IEnumerable<TechniqueType> techniques)
        {
            ExecutionId = executionId;
            TestProjectFullPath = testProjectFullPath;
            Techniques = techniques.ToArray();
        }

        public string ExecutionId { get; }
        public string TestProjectFullPath { get; }
        public IReadOnlyList<TechniqueType> Techniques { get; }
    }
}