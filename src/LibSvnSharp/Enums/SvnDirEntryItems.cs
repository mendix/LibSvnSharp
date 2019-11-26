using System;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [Flags]
    public enum SvnDirEntryItems
    {
        None = 0,

        /// <summary>An indication that you are interested in the Kind field</summary>
        Kind = svn_dirent_enum_t.SVN_DIRENT_KIND,

        /// <summary>An indication that you are interested in the @c size field</summary>
        Size = svn_dirent_enum_t.SVN_DIRENT_SIZE,

        /// <summary>An indication that you are interested in the @c has_props field</summary>
        HasProperties = svn_dirent_enum_t.SVN_DIRENT_HAS_PROPS,

        /// <summary>An indication that you are interested in the @c created_rev field</summary>
        Revision = svn_dirent_enum_t.SVN_DIRENT_CREATED_REV,

        /// <summary>An indication that you are interested in the @c time field</summary>
        Time = svn_dirent_enum_t.SVN_DIRENT_TIME,

        /// <summary>An indication that you are interested in the @c last_author field</summary>
        LastAuthor = svn_dirent_enum_t.SVN_DIRENT_LAST_AUTHOR,

        /// <summary>A combination of all the dirent fields at Subversion 1.5</summary>
        AllFieldsV15 = Kind | Size | HasProperties | Revision | Time | LastAuthor,

        /// <summary>The fields loaded if no other value is specified</summary>
        SvnListDefault = Kind | Time
    }
}
