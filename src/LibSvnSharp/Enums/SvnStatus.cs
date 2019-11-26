using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnStatus : uint
    {
        /// <summary>Zero value. Never used by Subversion</summary>
        Zero                                    = 0,

        /// <summary>does not exist</summary>
        None                                    = svn_wc_status_kind.svn_wc_status_none,

        /// <summary>is not a versioned thing in this wc</summary>
        NotVersioned                            = svn_wc_status_kind.svn_wc_status_unversioned,

        /// <summary>exists, but uninteresting</summary>
        Normal                                  = svn_wc_status_kind.svn_wc_status_normal,

        /// <summary>is scheduled for addition</summary>
        Added                                   = svn_wc_status_kind.svn_wc_status_added,

        /// <summary>under v.c., but is missing</summary>
        Missing                                 = svn_wc_status_kind.svn_wc_status_missing,

        /// <summary>scheduled for deletion</summary>
        Deleted                                 = svn_wc_status_kind.svn_wc_status_deleted,

        /// <summary>was deleted and then re-added</summary>
        Replaced                                = svn_wc_status_kind.svn_wc_status_replaced,

        /// <summary>text or props have been modified</summary>
        Modified                                = svn_wc_status_kind.svn_wc_status_modified,

        /// <summary>local mods received repos mods</summary>
        Merged                                  = svn_wc_status_kind.svn_wc_status_merged,

        /// <summary>local mods received conflicting repos mods</summary>
        Conflicted                              = svn_wc_status_kind.svn_wc_status_conflicted,

        /// <summary>is unversioned but configured to be ignored</summary>
        Ignored                                 = svn_wc_status_kind.svn_wc_status_ignored,

        /// <summary>an unversioned resource is in the way of the versioned resource</summary>
        Obstructed                              = svn_wc_status_kind.svn_wc_status_obstructed,

        /// <summary>an unversioned path populated by an svn:externals property</summary>
        External                                = svn_wc_status_kind.svn_wc_status_external,

        /// <summary>a directory doesn't contain a complete entries list</summary>
        Incomplete                              = svn_wc_status_kind.svn_wc_status_incomplete
    }
}