using System;
using System.Linq;
using FaultDetectorDotNet.Core.Suspiciousness;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Extension
{
    public static class SuspiciousnessServiceParametersFactory
    {
        public static SuspiciousnessServiceParameters Create(string testProjectFullPath,
            string[] spectrumBasedTechnics,
            string[] symmetryCoefficients,
            bool normalizatedTechnique)
        {
            var symmetryCoefficientsTypes = symmetryCoefficients
                .Select(value => (SymmetryCoefficientType)Enum.Parse(typeof(SymmetryCoefficientType), value))
                .ToArray();

            var spectrumBasedTechniqueTypes = spectrumBasedTechnics
                .Select(value => (TechniqueType)Enum.Parse(typeof(TechniqueType), value))
                .ToArray();

            var executionId = Guid.NewGuid().ToString();

            var suspiciousnessServiceParameters = new SuspiciousnessServiceParameters(executionId,
                testProjectFullPath,
                symmetryCoefficientsTypes,
                spectrumBasedTechniqueTypes,
                normalizatedTechnique
            );

            return suspiciousnessServiceParameters;
        }
    }
}