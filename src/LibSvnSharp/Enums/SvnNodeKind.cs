using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnNodeKind : uint
    {
        None                    = svn_node_kind_t.svn_node_none,
        File                    = svn_node_kind_t.svn_node_file,
        Directory               = svn_node_kind_t.svn_node_dir,
        Unknown                 = svn_node_kind_t.svn_node_unknown,
        SymbolicLink            = svn_node_kind_t.svn_node_symlink
    }
}
