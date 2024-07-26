namespace FaultDetectorDotNet.Core.Helpers
{
    public interface IProjectHelper
    {
        string BuildProject(string projectFilePath);
        bool IsTestProject(string projectFilePath);
    }
}