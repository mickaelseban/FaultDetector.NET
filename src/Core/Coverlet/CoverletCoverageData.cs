using System.Collections.Generic;

namespace FaultDetectorDotNet.Core.Coverlet
{
    public sealed class CoverletCoverageData
    {
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, MethodData>>>> Assembly
        {
            get;
            set;
        }
    }
}