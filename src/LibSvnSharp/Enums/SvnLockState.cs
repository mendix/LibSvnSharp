using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnLockState : uint
    {
        None                    = svn_wc_notify_lock_state_t.svn_wc_notify_lock_state_inapplicable,
        Unknown                 = svn_wc_notify_lock_state_t.svn_wc_notify_lock_state_unknown,
        Unchanged               = svn_wc_notify_lock_state_t.svn_wc_notify_lock_state_unchanged,
        Locked                  = svn_wc_notify_lock_state_t.svn_wc_notify_lock_state_locked,
        Unlocked                = svn_wc_notify_lock_state_t.svn_wc_notify_lock_state_unlocked
    }
}