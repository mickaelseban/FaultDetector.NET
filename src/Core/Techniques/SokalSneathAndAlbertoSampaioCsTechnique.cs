namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class SokalSneathAndAlbertoSampaioCsTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new SokalSneathAndAlbertoSampaioCSFormula();
        public override TechniqueType Type => TechniqueType.SokalSneathAndAlbertoSampaioCS;
    }
}