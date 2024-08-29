namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class RogerTanimotoAndAlbertoSampaioCsTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new RogerTanimotoAndAlbertoSampaioCSFormula();
        public override TechniqueType Type => TechniqueType.RogerTanimotoAndAlbertoSampaioCS;
    }
}