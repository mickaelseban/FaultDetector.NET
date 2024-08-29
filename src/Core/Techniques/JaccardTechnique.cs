namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class JaccardTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new JaccardFormula();
        public override TechniqueType Type => TechniqueType.Jaccard;
    }
}