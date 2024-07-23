using System;
using System.Linq;
using FaultDetectorDotNet.Core.Suspiciousness;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Tool
{
    public static class SuspiciousnessServiceParametersFactory
    {
        private static readonly TechniqueType[] AllTechniqueTypes = (TechniqueType[])Enum.GetValues(typeof(TechniqueType));

        public static SuspiciousnessServiceParameters Create(string testProjectFullPath,
            string[] spectrumBasedTechnics,
            bool allSpectrumBasedTechniques,
            string[] symmetryCoefficients,
            string executionId)
        {
            var techniques = TechniqueTypes(spectrumBasedTechnics, allSpectrumBasedTechniques);

            var symmetryCoefficientsTypes = symmetryCoefficients
                .Select(value => (SymmetryCoefficientType)Enum.Parse(typeof(SymmetryCoefficientType), value))
                .ToArray();

            return new SuspiciousnessServiceParameters(executionId, testProjectFullPath, symmetryCoefficientsTypes, techniques, false);
        }

        private static TechniqueType[] TechniqueTypes(string[] spectrumBasedTechnics, bool allSpectrumBasedTechniques)
        {
            if (allSpectrumBasedTechniques)
            {
                return AllTechniqueTypes;
            }

            return spectrumBasedTechnics
                .Select(x => (TechniqueType)Enum.Parse(typeof(TechniqueType), x))
                .ToArray();
        }
    }
}