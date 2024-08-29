using System;
using System.Linq;
using FaultDetectorDotNet.Core.Suspiciousness;
using FaultDetectorDotNet.Core.Techniques;

namespace FaultDetectorDotNet.Extension
{
    public static class SuspiciousnessServiceParametersFactory
    {
        public static SuspiciousnessServiceParameters Create(string testProjectFullPath,
            string[] spectrumBasedTechnics)
        {
            var spectrumBasedTechniqueTypes = spectrumBasedTechnics
                .Select(v => TechniqueType.FromValue(Convert.ToInt32(v)))
                .ToArray();

            var executionId = Guid.NewGuid().ToString();

            var suspiciousnessServiceParameters = new SuspiciousnessServiceParameters(executionId,
                testProjectFullPath,
                spectrumBasedTechniqueTypes
            );

            return suspiciousnessServiceParameters;
        }
    }
}