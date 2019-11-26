using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnRevisionType : uint
    {
        None                    = svn_opt_revision_kind.svn_opt_revision_unspecified,
        Number                  = svn_opt_revision_kind.svn_opt_revision_number,
        Time                    = svn_opt_revision_kind.svn_opt_revision_date,
        Committed               = svn_opt_revision_kind.svn_opt_revision_committed,
        Previous                = svn_opt_revision_kind.svn_opt_revision_previous,
        Base                    = svn_opt_revision_kind.svn_opt_revision_base,
        Working                 = svn_opt_revision_kind.svn_opt_revision_working,
        Head                    = svn_opt_revision_kind.svn_opt_revision_head
    }
}
