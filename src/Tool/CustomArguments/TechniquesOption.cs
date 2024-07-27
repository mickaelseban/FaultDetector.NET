using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Tool.CustomArguments
{
    public sealed class TechniquesOption : Option<IEnumerable<TechniqueType>>
    {
        public TechniquesOption() : base(
            ["--techniques", "-t"],
            description: $"Fault Localization Techniques: ({TechniquesList()})",
            parseArgument: ParseTechniques)
        {
            SetDefaultValue(Default);
        }

        private static IEnumerable<TechniqueType> Default { get; } = [TechniqueType.Tarantula];

        private static string TechniquesList()
        {
            var techniques = Enum.GetValues<TechniqueType>()
                .Select(type => $"{(int)type} - {type}")
                .ToArray();

            return string.Join(" | ", techniques);
        }

        private static IEnumerable<TechniqueType> ParseTechniques(ArgumentResult result)
        {
            var techniques = new HashSet<TechniqueType>();

            foreach (var token in result.Tokens)
            {
                var values = token.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in values)
                {
                    if (int.TryParse(value, out var intValue) && Enum.IsDefined(typeof(TechniqueType), intValue))
                    {
                        techniques.Add((TechniqueType)intValue);
                    }
                    else
                    {
                        result.ErrorMessage = $"Cannot parse '{value}' as a valid TechniqueType.";
                        return techniques;
                    }
                }
            }

            return techniques.ToArray();
        }
    }
}