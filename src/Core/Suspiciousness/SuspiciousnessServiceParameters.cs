using System.Collections.Generic;
using System.Linq;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public class SuspiciousnessServiceParameters
    {
        public SuspiciousnessServiceParameters(string executionId,
            string testProjectFullPath,
            IEnumerable<SymmetryCoefficientType> symmetryCoefficients,
            IEnumerable<TechniqueType> techniques,
            bool normalizatedTechnique)
        {
            ExecutionId = executionId;
            TestProjectFullPath = testProjectFullPath;
            NormalizatedTechnique = normalizatedTechnique;
            SymmetryCoefficients = symmetryCoefficients.ToArray();
            Techniques = techniques.ToArray();
        }

        public string ExecutionId { get; }
        public string TestProjectFullPath { get; }
        public bool NormalizatedTechnique { get; }
        public IReadOnlyList<SymmetryCoefficientType> SymmetryCoefficients { get; }
        public IReadOnlyList<TechniqueType> Techniques { get; }
    }
}