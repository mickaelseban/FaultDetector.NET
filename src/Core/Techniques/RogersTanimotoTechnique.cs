namespace FaultDetectorDotNet.Core.Techniques
{
    public sealed class RogersTanimotoTechnique : TechniqueBase
    {
        public override ISpectrumBasedFormula Formula { get; } = new RogersTanimotoFormula();
        public override TechniqueType Type => TechniqueType.RogersTanimoto;
    }
}