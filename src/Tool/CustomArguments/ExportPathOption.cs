using System.CommandLine;
using System.IO;

namespace FaultDetectorDotNet.Tool.CustomArguments
{
    public sealed class ExportPathOption : Option<string>
    {
        public ExportPathOption() : base(["--exportPath", "-e"], "The path to export the results")
        {
            AddValidator(optionResult =>
            {
                var valueOrDefault = optionResult.GetValueOrDefault<string>();
                if (!PathValidator.IsValidPath(valueOrDefault))
                {
                    optionResult.ErrorMessage = $"The option: {Name} is specified and it must be a valid path";
                }
                else if (!Directory.Exists(valueOrDefault))
                {
                    optionResult.ErrorMessage = $"The option: {Name} is specified and it must be a path for and existent directory";
                }
            });
        }
    }
}