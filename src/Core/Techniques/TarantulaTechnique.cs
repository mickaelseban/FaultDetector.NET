namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class TarantulaTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new TarantulaFormula();
        public override TechniqueType Type => TechniqueType.Tarantula;
    }
}