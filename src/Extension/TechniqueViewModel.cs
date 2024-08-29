using FaultDetectorDotNet.Core.Techniques;

namespace FaultDetectorDotNet.Extension
{
    public class TechniqueViewModel
    {
        public TechniqueType Tarantula { get; } = TechniqueType.Tarantula;
        public TechniqueType Ochiai { get; } = TechniqueType.Ochiai;
        public TechniqueType Jaccard { get; } = TechniqueType.Jaccard;
        public TechniqueType DStar { get; } = TechniqueType.DStar;
        public TechniqueType Kulczynski { get; } = TechniqueType.Kulczynski;
        public TechniqueType RogersTanimoto { get; } = TechniqueType.RogersTanimoto;
        public TechniqueType MickaelSeban { get; } = TechniqueType.MickaelSeban;
        public TechniqueType TarantulaAndAlbertoSampaioCs { get; } = TechniqueType.TarantulaAndAlbertoSampaioCS;
        public TechniqueType SokalSneathAndAlbertoSampaioCs { get; } = TechniqueType.SokalSneathAndAlbertoSampaioCS;
        public TechniqueType SimpleMatchingAndAlbertoSampaioCs { get; } = TechniqueType.SimpleMatchingAndAlbertoSampaioCS;
        public TechniqueType RogerTanimotoAndAlbertoSampaioCs { get; } = TechniqueType.RogerTanimotoAndAlbertoSampaioCS;
    }
}