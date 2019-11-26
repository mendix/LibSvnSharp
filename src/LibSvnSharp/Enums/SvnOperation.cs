using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnOperation : uint
    {
        None = svn_wc_operation_t.svn_wc_operation_none,
        Update = svn_wc_operation_t.svn_wc_operation_update,
        Switch = svn_wc_operation_t.svn_wc_operation_switch,
        Merge = svn_wc_operation_t.svn_wc_operation_merge
    }
}
