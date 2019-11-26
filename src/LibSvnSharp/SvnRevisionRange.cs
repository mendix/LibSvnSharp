using System;
using System.Diagnostics;

namespace LibSvnSharp
{
    [DebuggerDisplay("Range={StartRevision}-{EndRevision}")]
    public class SvnRevisionRange : IEquatable<SvnRevisionRange>
    {
        public SvnRevisionRange(long start, long end)
        {
            StartRevision = new SvnRevision(start);
            EndRevision = new SvnRevision(end);
        }

        public SvnRevisionRange(SvnRevision start, SvnRevision end)
        {
            StartRevision = start ?? throw new ArgumentNullException(nameof(start));
            EndRevision = end ?? throw new ArgumentNullException(nameof(end));
        }

        public SvnRevision StartRevision { get; }

        public SvnRevision EndRevision { get; }

        public SvnRevisionRange Reverse()
        {
            return new SvnRevisionRange(EndRevision, StartRevision);
        }

        /// <summary>Creates a SvnRevision from {revision-1:revision}</summary>
        public static SvnRevisionRange FromRevision(long revision)
        {
            return new SvnRevisionRange(revision - 1, revision);
        }

        public override string ToString()
        {
            return $"{StartRevision}-{EndRevision}";
        }

        public override int GetHashCode()
        {
            return StartRevision.GetHashCode() ^ (EndRevision.GetHashCode() << 3);
        }

        public override bool Equals(object other)
        {
            return Equals(other as SvnRevisionRange);
        }

        public bool Equals(SvnRevisionRange range)
        {
            if (range == null)
                return false;

            return StartRevision.Equals(range.StartRevision) &&
                   EndRevision.Equals(range.EndRevision);
        }

        public static SvnRevisionRange operator -(SvnRevisionRange @from)
        {
            return @from?.Reverse();
        }

        public static SvnRevisionRange None { get; } = new SvnRevisionRange(SvnRevision.None, SvnRevision.None);
    }
}
