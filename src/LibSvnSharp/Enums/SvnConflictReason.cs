using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnConflictReason : uint
    {
        /// <summary>local edits are already present</summary>
        Edited = svn_wc_conflict_reason_t.svn_wc_conflict_reason_edited,
        /// <summary>another object is in the way</summary>
        Obstructed = svn_wc_conflict_reason_t.svn_wc_conflict_reason_obstructed,
        /// <summary>object is already schedule-delete</summary>
        Deleted = svn_wc_conflict_reason_t.svn_wc_conflict_reason_deleted,
        /// <summary>object is unknown or missing</summary>
        Missing = svn_wc_conflict_reason_t.svn_wc_conflict_reason_missing,
        /// <summary>object is unversioned</summary>
        NotVersioned = svn_wc_conflict_reason_t.svn_wc_conflict_reason_unversioned,
        /// <summary>object is already added or schedule-add</summary>
        Added = svn_wc_conflict_reason_t.svn_wc_conflict_reason_added,

        Replaced = svn_wc_conflict_reason_t.svn_wc_conflict_reason_replaced,
        MovedAway = svn_wc_conflict_reason_t.svn_wc_conflict_reason_moved_away,
        MovedHere = svn_wc_conflict_reason_t.svn_wc_conflict_reason_moved_here,
    }
}
