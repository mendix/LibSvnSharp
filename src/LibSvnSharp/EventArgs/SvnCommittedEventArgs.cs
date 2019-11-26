using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnCommittedEventArgs : SvnCommitResult
    {
        internal static SvnCommittedEventArgs Create(SvnClientContext client, svn_commit_info_t commitInfo, AprPool pool)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (commitInfo == null || (commitInfo.revision <= 0L))
                return null;

            return new SvnCommittedEventArgs(commitInfo, pool);
        }

        SvnCommittedEventArgs(svn_commit_info_t commitInfo, AprPool pool)
            : base(commitInfo, pool)
        {
        }
    }
}
