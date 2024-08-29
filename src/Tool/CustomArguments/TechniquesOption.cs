using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using FaultDetectorDotNet.Core.Techniques;

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
            var techniques = TechniqueType.List()
                .Select(type => $"{type.Value} - {type.DisplayName}")
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
                    if (int.TryParse(value, out var intValue))
                    {
                        var technique = TechniqueType.FromValue(intValue);
                        if (technique != null)
                        {
                            techniques.Add(technique);
                        }
                        else
                        {
                            result.ErrorMessage = $"Cannot parse '{value}' as a valid TechniqueType.";
                            return techniques;
                        }
                    }
                    else
                    {
                        var technique = TechniqueType.FromName(value.Trim());
                        if (technique != null)
                        {
                            techniques.Add(technique);
                        }
                        else
                        {
                            result.ErrorMessage = $"Cannot parse '{value}' as a valid TechniqueType.";
                            return techniques;
                        }
                    }
                }
            }

            return techniques.ToArray();
        }
    }
}