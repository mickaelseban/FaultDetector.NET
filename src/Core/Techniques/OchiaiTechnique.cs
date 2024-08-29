namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class OchiaiTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new OchiaiFormula();
        public override TechniqueType Type => TechniqueType.Ochiai;
    }
}