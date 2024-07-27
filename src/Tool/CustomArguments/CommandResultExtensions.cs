using System.CommandLine;
using System.CommandLine.Parsing;

namespace FaultDetectorDotNet.Tool.CustomArguments
{
    public static class CommandResultExtensions
    {
        public static T GetValueOrDefaultForOption<T>(this CommandResult commandResult, Option<T> option)
        {
            T value = default;
            try
            {
                value = commandResult.GetValueForOption(option);
            }
            catch
            {
                // ignored
            }

            return value;
        }
    }
}