using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FaultDetectorDotNet.Core.Coverlet
{
    public static class CoverletService
    {
        public static CoverletCoverageData ReadReport(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var json = streamReader.ReadToEnd();
                    var coverageData =
                        JsonConvert
                            .DeserializeObject<
                                Dictionary<string,
                                    Dictionary<string, Dictionary<string, Dictionary<string, MethodData>>>>>(json);
                    return new CoverletCoverageData { Assembly = coverageData };
                }
            }
        }
    }
}