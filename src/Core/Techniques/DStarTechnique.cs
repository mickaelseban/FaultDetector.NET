namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class DStarTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new DStarFormula();
        public override TechniqueType Type => TechniqueType.DStar;
    }
}