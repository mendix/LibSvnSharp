using System;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [Flags]
    public enum SvnCommitTypes
    {
        None = 0,
        Added = svn_client_commit_item_enum_t.SVN_CLIENT_COMMIT_ITEM_ADD,
        Deleted = svn_client_commit_item_enum_t.SVN_CLIENT_COMMIT_ITEM_DELETE,
        ContentModified = svn_client_commit_item_enum_t.SVN_CLIENT_COMMIT_ITEM_TEXT_MODS,
        PropertiesModified = svn_client_commit_item_enum_t.SVN_CLIENT_COMMIT_ITEM_PROP_MODS,
        Copied = svn_client_commit_item_enum_t.SVN_CLIENT_COMMIT_ITEM_IS_COPY,
        HasLockToken = svn_client_commit_item_enum_t.SVN_CLIENT_COMMIT_ITEM_LOCK_TOKEN,
        MovedHere = svn_client_commit_item_enum_t.SVN_CLIENT_COMMIT_ITEM_MOVED_HERE,
    }
}
