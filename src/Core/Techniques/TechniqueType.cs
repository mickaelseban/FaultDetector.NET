using System;
using System.Collections.Generic;
using System.Linq;

namespace FaultDetectorDotNet.Core.Techniques
{
    public class TechniqueType 
    {
        protected bool Equals(TechniqueType other)
        {
            return Name == other.Name && Value == other.Value && DisplayName == other.DisplayName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((TechniqueType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Value;
                hashCode = (hashCode * 397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static readonly TechniqueType Tarantula = new TechniqueType(nameof(Tarantula), 1, "Tarantula");
        public static readonly TechniqueType Ochiai = new TechniqueType(nameof(Ochiai), 2, "Ochiai");
        public static readonly TechniqueType Jaccard = new TechniqueType(nameof(Jaccard), 3, "Jaccard");
        public static readonly TechniqueType DStar = new TechniqueType(nameof(DStar), 4, "D2 (DStar)");
        public static readonly TechniqueType Kulczynski = new TechniqueType(nameof(Kulczynski), 5, "Kulczynski");
        public static readonly TechniqueType RogersTanimoto = new TechniqueType(nameof(RogersTanimoto), 6, "Rogers-Tanimoto");
        public static readonly TechniqueType MickaelSeban = new TechniqueType(nameof(MickaelSeban), 7, "Seban Normalization");
        public static readonly TechniqueType TarantulaAndAlbertoSampaioCS = new TechniqueType(nameof(TarantulaAndAlbertoSampaioCS), 
            8, "Tarantula & Sampaio");

        public static readonly TechniqueType SokalSneathAndAlbertoSampaioCS = 
            new TechniqueType(nameof(SokalSneathAndAlbertoSampaioCS), 9, "Sokal Sneath & Sampaio");

        public static readonly TechniqueType RogerTanimotoAndAlbertoSampaioCS = new TechniqueType(nameof(RogerTanimotoAndAlbertoSampaioCS),
            10, "Roger-Tanimoto & Sampaio");

        public static readonly TechniqueType SimpleMatchingAndAlbertoSampaioCS = new TechniqueType(nameof(SimpleMatchingAndAlbertoSampaioCS),
            11, "Simple Matching & Sampaio");
        
        private TechniqueType(string name, int value, string displayName)
        {
            Name = name;
            Value = value;
            DisplayName = displayName;
        }

        public string Name { get; }
        public int Value { get; }
        public string DisplayName { get; }

        public static IEnumerable<TechniqueType> List()
        {
            return new[]
            {
                Tarantula, Ochiai, Jaccard, DStar, Kulczynski, RogersTanimoto, MickaelSeban,
                TarantulaAndAlbertoSampaioCS, SokalSneathAndAlbertoSampaioCS,
                RogerTanimotoAndAlbertoSampaioCS, 
                SimpleMatchingAndAlbertoSampaioCS
            };
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static TechniqueType FromValue(int value)
        {
            return List().FirstOrDefault(t => t.Value == value);
        }

        public static TechniqueType FromName(string name)
        {
            return List().FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public static TechniqueType FromDisplayName(string displayName)
        {
            return List().FirstOrDefault(t =>
                string.Equals(t.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
        }
    }
}