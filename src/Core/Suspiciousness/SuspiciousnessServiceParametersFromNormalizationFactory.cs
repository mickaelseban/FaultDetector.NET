using System.Collections.Generic;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public static class SuspiciousnessServiceParametersFromNormalizationFactory
    {
        private static readonly IEnumerable<SymmetryCoefficientType> SymmetryCoefficients =
            new[] { SymmetryCoefficientType.Default };

        private static readonly IEnumerable<TechniqueType> Techniques = new List<TechniqueType>
        {
            TechniqueType.Tarantula,
            TechniqueType.Jaccard,
            TechniqueType.Kulczynski,
            TechniqueType.Ochiai,
            TechniqueType.RogersTanimoto
        };

        public static SuspiciousnessServiceParameters Create(SuspiciousnessServiceParameters otherParameters)
        {
            return new SuspiciousnessServiceParameters(otherParameters.ExecutionId,
                otherParameters.TestProjectFullPath,
                SymmetryCoefficients,
                Techniques,
                otherParameters.NormalizatedTechnique
            );
        }
    }
}