using System;
using System.Collections.Generic;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public sealed class SvnUpdateResult : SvnCommandResult
    {
        SvnUpdateResult(long revision)
        {
            if (revision < 0)
                revision = -1;

            Revision = revision;
        }

        public SvnUpdateResult(IDictionary<string, SvnUpdateResult> resultMap, long revision)
        {
            if (resultMap == null)
                throw new ArgumentNullException(nameof(resultMap));
            if (revision < 0)
                revision = -1;

            Revision = revision;
            ResultMap = resultMap;
        }

        internal static SvnUpdateResult Create(SvnClient client, SvnClientArgs args, long revision)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return new SvnUpdateResult(revision);
        }

        internal SvnUpdateResult(ICollection<string> paths, ICollection<long> revisions, long revision)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (revisions == null)
                throw new ArgumentNullException(nameof(revisions));
            if (paths.Count != revisions.Count)
                throw new ArgumentException(SharpSvnStrings.PathCountDoesNotMatchRevisions, nameof(paths));

            if (revision < 0)
                revision = -1;

            Revision = revision;
            ResultMap = new SortedList<string, SvnUpdateResult>();
            var ePath = paths.GetEnumerator();
            var eRev = revisions.GetEnumerator();

            while (ePath.MoveNext() && eRev.MoveNext())
            {
                if (!(ResultMap.ContainsKey(ePath.Current)))
                    ResultMap.Add(ePath.Current, new SvnUpdateResult(eRev.Current));
            }
        }

        public bool HasRevision => Revision >= 0;

        public long Revision { get; }

        public bool HasResultMap => (ResultMap != null);

        public IDictionary<string, SvnUpdateResult> ResultMap { get; }

        public override int GetHashCode()
        {
            return Revision.GetHashCode();
        }
    }
}
