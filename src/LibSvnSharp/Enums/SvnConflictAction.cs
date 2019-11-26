using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnConflictAction : uint
    {
        /// <summary>Attempting to change text or props</summary>
        Edit = svn_wc_conflict_action_t.svn_wc_conflict_action_edit,
        /// <summary>Attempting to add object</summary>
        Add = svn_wc_conflict_action_t.svn_wc_conflict_action_add,
        /// <summary>Attempting to delete object</summary>
        Delete = svn_wc_conflict_action_t.svn_wc_conflict_action_delete,

        Replace = svn_wc_conflict_action_t.svn_wc_conflict_action_replace,
    }
}
