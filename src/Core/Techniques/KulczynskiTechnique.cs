namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class KulczynskiTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new KulczynskiFormula();
        public override TechniqueType Type => TechniqueType.Kulczynski;
    }
}