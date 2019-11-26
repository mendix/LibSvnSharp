using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnDepth
    {
        /// <summary>Depth undetermined or ignored</summary>
        Unknown                         = svn_depth_t.svn_depth_unknown,

        // Not supported in 1.5 client api
        /// <summary>Exclude (remove, whatever) directory D</summary>
        Exclude                         = svn_depth_t.svn_depth_exclude,

        /// <summary>
        /// Just the named directory D, no entries. Updates will not pull in any
        /// files or subdirectories not already present
        /// </summary>
        Empty                           = svn_depth_t.svn_depth_empty,

        /// <summary>
        /// D + its file children, but not subdirs.  Updates will pull in any files
        /// not already present, but not subdirectories.
        /// </summary>
        Files                           = svn_depth_t.svn_depth_files,

        /// <summary>
        /// D + immediate children (D and its entries). Updates will pull in any
        /// files or subdirectories not already present; those subdirectories'
        /// this_dir entries will have depth-empty.
        /// </summary>
        Children                        = svn_depth_t.svn_depth_immediates,

        /// <summary>
        /// D + all descendants (full recursion from D). Updates will pull in any
        /// files or subdirectories not already present; those subdirectories'
        /// this_dir entries will have depth-infinity. Equivalent to the pre-1.5
        /// default update behavior.
        /// </summary>
        Infinity                        = svn_depth_t.svn_depth_infinity,
    }
}
