namespace FaultDetectorDotNet.Core.Techniques
{
    public interface ISpectrumBasedFormula
    {
        double Calculate(FormulaRequest request);
    }
}