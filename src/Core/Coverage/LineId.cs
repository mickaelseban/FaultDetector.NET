namespace FaultDetectorDotNet.Core.Coverage
{
    public sealed class LineId
    {
        public LineId(string assemblyName, string className, string filePath, int lineNumber)
        {
            AssemblyName = assemblyName.Replace(".dll", string.Empty);
            ClassName = className;
            FilePath = filePath;
            LineNumber = lineNumber;
        }

        public string AssemblyName { get; }
        public string ClassName { get; }
        public string FilePath { get; }
        public int LineNumber { get; }

        private bool Equals(LineId other)
        {
            return AssemblyName == other.AssemblyName && ClassName == other.ClassName && LineNumber == other.LineNumber;
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

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((LineId)obj);
        }
        public override string ToString()
        {
            return $"{LineNumber.ToString(),4} - {ClassName}";
        }

        public static implicit operator string(LineId lineId)
        {
            return lineId.ToString();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AssemblyName != null ? AssemblyName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LineNumber;
                return hashCode;
            }
        }
    }
}