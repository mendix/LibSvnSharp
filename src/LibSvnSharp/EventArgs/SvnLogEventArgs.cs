using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public class SvnLogEventArgs : SvnLoggingEventArgs
    {
        internal SvnLogEventArgs(svn_log_entry_t entry, int mergeLevel, Uri logOrigin, AprPool pool)
            : base(entry, pool)
        {
            HasChildren = entry.has_children;
            MergeLogNestingLevel = mergeLevel;
            LogOrigin = logOrigin;
        }

        /// <summary>Gets the log origin LibSvnSharp used for retrieving the log</summary>
        public Uri LogOrigin { get; }

        /// <summary>Set to true when the following items are merged-child items of this item.</summary>
        public bool HasChildren { get; }

        /// <summary>Gets the nesting level of the logs via merges</summary>
        public int MergeLogNestingLevel { get; }
    }
}
