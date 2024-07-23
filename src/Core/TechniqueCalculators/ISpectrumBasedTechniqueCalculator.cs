namespace FaultDetectorDotNet.Core.TechniqueCalculators
{
    public interface ISpectrumBasedTechniqueCalculator
    {
        TechniqueType Type { get; }
        double Calculate((int passCount, int failCount) counts, int totalPassedTests, int totalFailedTests, double? symmetryCoefficient);
    }
}