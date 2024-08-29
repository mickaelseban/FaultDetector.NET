using System.Linq;
using FaultDetectorDotNet.Core.Coverage;
using FaultDetectorDotNet.Core.Techniques;

namespace FaultDetectorDotNet.Core.Suspiciousness
{
    public static class SuspiciousnessCalculator
    {
        private static readonly ITechnique[] Techniques =
        {
            new TarantulaTechnique(),
            new OchiaiTechnique(),
            new JaccardTechnique(),
            new DStarTechnique(),
            new KulczynskiTechnique(),
            new RogersTanimotoTechnique(),
            new MickaelSebanTechnique(),
            new TarantulaAndAlbertoSampaioCsTechnique(),
            new SokalSneathAndAlbertoSampaioCsTechnique(),
            new SimpleMatchingAndAlbertoSampaioCsTechnique(),
            new RogerTanimotoAndAlbertoSampaioCsTechnique()
        };

        public static SuspiciousnessResult Calculate(CoverageAggregatedModel coverageAggregated,
            SuspiciousnessServiceParameters parameters)
        {
            var result = new SuspiciousnessResult();

            foreach (var requestTechnique in parameters.Techniques)
            {

                var techniqueResult = Techniques
                    .Single(techniqueCalculator =>
                    {
                        var s = techniqueCalculator.Type.Equals(requestTechnique);
                        return s;
                    })
                    .Calculate(coverageAggregated);

                result.Techniques[requestTechnique] = techniqueResult;
            }

            return result;
        }
    }
}