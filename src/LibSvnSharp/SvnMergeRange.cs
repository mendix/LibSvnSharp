using System;
using System.Diagnostics;
using System.Globalization;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [DebuggerDisplay("Range=r{Start}-{End}, Inheritable={Inheritable}")]
    public sealed class SvnMergeRange : SvnRevisionRange
    {
        public SvnMergeRange(long start, long end, bool inheritable)
            : base(start, end)
        {
            Inheritable = inheritable;
        }

        public long Start => StartRevision.Revision;

        public long End => EndRevision.Revision;

        public bool Inheritable { get; }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return Start.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "r{0}-{1}", Start, End);
        }

        internal unsafe svn_merge_range_t.__Internal* AllocMergeRange(AprPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var range = (svn_merge_range_t.__Internal*) pool.AllocCleared(
                sizeof(svn_merge_range_t.__Internal));

            range->start = (int) Start;
            range->end = (int) End;
            range->inheritable = Inheritable ? 1 : 0;

            return range;
        }
    }
}
