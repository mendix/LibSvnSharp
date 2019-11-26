using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public class SvnCommitResult : SvnCommandResult
    {
        internal unsafe SvnCommitResult(svn_commit_info_t commitInfo, AprPool pool)
        {
            if (commitInfo == null)
                throw new ArgumentNullException(nameof(commitInfo));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            Revision = commitInfo.revision;

            long when = 0;
            svn_error_t err = svn_time.svn_time_from_cstring(ref when, commitInfo.date, pool.Handle); // pool is not used at this time (might be for errors in future versions)

            if (err == null)
                Time = SvnBase.DateTimeFromAprTime(when);
            else
            {
                svn_error.svn_error_clear(err);
                Time = DateTime.MinValue;
            }

            Author = SvnBase.Utf8_PtrToString(commitInfo.author);

            PostCommitError = commitInfo.post_commit_err != null
                            ? SvnBase.Utf8_PtrToString(commitInfo.post_commit_err)
                                     .Replace("\n", Environment.NewLine)
                                     .Replace("\r\r", "\r")
                            : null;

            if (commitInfo.repos_root != null)
                RepositoryRoot = SvnBase.Utf8_PtrToUri(commitInfo.repos_root, SvnNodeKind.Directory);
        }

        public long Revision { get; }

        public DateTime Time { get; }

        public string Author { get; }

        public Uri RepositoryRoot { get; }

        /// <summary>error message from post-commit hook, or NULL</summary>
        public string PostCommitError { get; }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return Revision.GetHashCode();
        }
    }
}
