using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnSchedule : uint
    {
        /// <summary> Nothing special here </summary>
        Normal                          = svn_wc_schedule_t.svn_wc_schedule_normal,

        /// <summary> Slated for addition </summary>
        Add                             = svn_wc_schedule_t.svn_wc_schedule_add,

        /// <summary> Slated for deletion </summary>
        Delete                          = svn_wc_schedule_t.svn_wc_schedule_delete,

        /// <summary> Slated for replacement (delete + add) </summary>
        Replace                         = svn_wc_schedule_t.svn_wc_schedule_replace
    }
}
