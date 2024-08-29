namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class TarantulaAndAlbertoSampaioCsTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new TarantulaAndAlbertoSampaioCsFormula();
        public override TechniqueType Type => TechniqueType.TarantulaAndAlbertoSampaioCS;
    }
}