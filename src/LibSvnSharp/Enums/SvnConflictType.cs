using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnConflictType : uint
    {
        /// <summary>textual conflict (on a file)</summary>
        Content = svn_wc_conflict_kind_t.svn_wc_conflict_kind_text,
        /// <summary>property conflict (on a file or dir)</summary>
        Property = svn_wc_conflict_kind_t.svn_wc_conflict_kind_property,
        /// <summary>tree conflict (on a dir)</summary>
        Tree = svn_wc_conflict_kind_t.svn_wc_conflict_kind_tree,

        /// <summary>Deprecated: Use .Tree</summary>
        TreeConflict = Tree,
    }
}
