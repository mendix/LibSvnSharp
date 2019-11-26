using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnAccept
    {
        /// <summary>Don't resolve the conflict now.  Let subversion mark the path
        /// 'conflicted', so user can run 'svn resolved' later</summary>
        Postpone                        = svn_wc_conflict_choice_t.svn_wc_conflict_choose_postpone,

        /// <summary>Choose the base file</summary>
        Base                            = svn_wc_conflict_choice_t.svn_wc_conflict_choose_base,

        /// <summary>Choose the incoming file</summary>
        TheirsFull                      = svn_wc_conflict_choice_t.svn_wc_conflict_choose_theirs_full,

        /// <summary>Choose the local file</summary>
        MineFull                        = svn_wc_conflict_choice_t.svn_wc_conflict_choose_mine_full,


        /// <summary>Choose their on all conflicts; further use auto-merged</summary>
        Theirs                          = svn_wc_conflict_choice_t.svn_wc_conflict_choose_theirs_conflict,
        /// <summary>Choose local version on all conflicts; further use auto-merged</summary>
        Mine                            = svn_wc_conflict_choice_t.svn_wc_conflict_choose_mine_conflict,
        /// <summary>Choose the 'merged file'. The result file of the automatic merging; possibly with local edits</summary>
        Merged                          = svn_wc_conflict_choice_t.svn_wc_conflict_choose_merged,

        /// <summary>Alias for Merged</summary>
        Working                         = Merged,

        /// <summary>Value not determined yet</summary>
        Unspecified                     = svn_wc_conflict_choice_t.svn_wc_conflict_choose_unspecified,
    }
}
