using System.CommandLine;
using System.CommandLine.Parsing;

namespace FaultDetectorDotNet.Tool.CustomArguments
{
    public sealed class TestProjectFullPathArgument : Argument<string>
    {
        public TestProjectFullPathArgument()
        {
            Name = "test-project-path";
            Description = "The test project full path";
            AddValidator(Validator);
        }

        private void Validator(ArgumentResult result)
        {
            var argument = result.GetValueForArgument(this);

            if (string.IsNullOrEmpty(argument))
            {
                result.ErrorMessage = $"{Name} is required";
            }
            else if (!Core.Helpers.ProjectHelper.IsTestProject(argument))
            {
                result.ErrorMessage = $"{Name} is not a valid test project";
            }
        }
    }
}