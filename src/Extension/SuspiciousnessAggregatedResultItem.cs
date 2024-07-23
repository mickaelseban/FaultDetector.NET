namespace FaultDetectorDotNet.Extension
{
    public class NormalizatedSuspiciousnessItem
    {
        public string Assembly { get; set; }
        public string File { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public int Line { get; set; }
        public double Score { get; set; }
    }
}