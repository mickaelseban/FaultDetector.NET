namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class SimpleMatchingAndAlbertoSampaioCsTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new SimpleMatchingAndAlbertoSampaioCSFormula();
        public override TechniqueType Type => TechniqueType.SimpleMatchingAndAlbertoSampaioCS;
    }
}