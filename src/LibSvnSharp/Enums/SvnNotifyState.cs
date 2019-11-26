using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnNotifyState : uint
    {
        None                    = svn_wc_notify_state_t.svn_wc_notify_state_inapplicable,
        Unknown                 = svn_wc_notify_state_t.svn_wc_notify_state_unknown,
        Unchanged               = svn_wc_notify_state_t.svn_wc_notify_state_unchanged,
        Missing                 = svn_wc_notify_state_t.svn_wc_notify_state_missing,
        Obstructed              = svn_wc_notify_state_t.svn_wc_notify_state_obstructed,
        Changed                 = svn_wc_notify_state_t.svn_wc_notify_state_changed,
        Merged                  = svn_wc_notify_state_t.svn_wc_notify_state_merged,
        Conflicted              = svn_wc_notify_state_t.svn_wc_notify_state_conflicted,
        SourceMissing           = svn_wc_notify_state_t.svn_wc_notify_state_source_missing,
    }
}