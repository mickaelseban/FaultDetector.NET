using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using FaultDetectorDotNet.Core.SymmetryCoefficientCalculators;
using FaultDetectorDotNet.Core.TechniqueCalculators;

namespace FaultDetectorDotNet.Tool.CustomArguments
{
    public sealed class SymmetryCoefficientsOption : Option<IEnumerable<SymmetryCoefficientType>>
    {
        public SymmetryCoefficientsOption() : base(
            new[] { "--coefficients", "-c" },
            description: $"Symmetry Coefficient: ({TechniquesList()})",
            parseArgument: ParseTechniques)
        {
            SetDefaultValue(Default);
        }

        private static IEnumerable<SymmetryCoefficientType> Default { get; } = [SymmetryCoefficientType.Default];

        private static string TechniquesList()
        {
            var techniques = Enum.GetValues<SymmetryCoefficientType>()
                .Select(type => $"{(int)type} - {type}")
                .ToArray();

            return string.Join(" | ", techniques);
        }

        private static IEnumerable<SymmetryCoefficientType> ParseTechniques(ArgumentResult result)
        {
            var techniques = new HashSet<SymmetryCoefficientType>();

            foreach (var token in result.Tokens)
            {
                var values = token.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in values)
                {
                    if (int.TryParse(value, out var intValue) && Enum.IsDefined(typeof(TechniqueType), intValue))
                    {
                        techniques.Add((SymmetryCoefficientType)intValue);
                    }
                    else
                    {
                        result.ErrorMessage = $"Cannot parse '{value}' as a valid {nameof(SymmetryCoefficientType)}.";
                        return techniques;
                    }
                }
            }

            return techniques.ToArray();
        }
    }
}