using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public unsafe class SvnStatusEventArgs : SvnCancelEventArgs
    {
        sbyte* _pPath;
        svn_client_status_t _status;
        AprPool _pool;
        SvnClientContext _client;

        string _path;
        string _fullPath;
        string _lastChangeAuthor;
        Uri _reposRoot;
        Uri _uri;
        string _repositoryId;
        SvnLockInfo _localLock;
        string _changelist;
        SvnLockInfo _reposLock;
        SvnWorkingCopyInfo _wcInfo;
        string _oodLastCommitAuthor;
        string _movedTo;
        string _movedFrom;

        internal SvnStatusEventArgs(sbyte* path, svn_client_status_t status, SvnClientContext client, AprPool pool)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (status == null)
                throw new ArgumentNullException(nameof(status));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _pPath = path;
            _status = status;
            _pool = pool;
            _client = client;

            Versioned = status.versioned;
            Conflicted = status.conflicted;
            NodeKind = (SvnNodeKind)status.kind;
            FileLength = (status.filesize >= 0) ? status.filesize : -1;

            LocalNodeStatus = (SvnStatus)status.node_status;
            LocalTextStatus = (SvnStatus)status.text_status;
            LocalPropertyStatus = (SvnStatus)status.prop_status;

            Wedged = status.wc_is_locked;
            LocalCopied = status.copied;
            Revision = status.revision;

            LastChangeRevision = status.changed_rev;
            LastChangeTime = SvnBase.DateTimeFromAprTime(status.changed_date);

            Switched = status.switched;
            IsFileExternal = status.file_external;
            Depth = (SvnDepth)status.depth;

            RemoteNodeStatus = (SvnStatus)status.repos_node_status;
            RemoteTextStatus = (SvnStatus)status.repos_text_status;
            RemotePropertyStatus = (SvnStatus)status.repos_prop_status;

            RemoteUpdateRevision = status.ood_changed_rev;
            if (status.ood_changed_rev != -1)
            {
                RemoteUpdateCommitTime = SvnBase.DateTimeFromAprTime(status.ood_changed_date);
                RemoteUpdateNodeKind = (SvnNodeKind)status.ood_kind;
            }
        }

        /// <summary>Gets the recorded node type of this node</summary>
        public SvnNodeKind NodeKind { get; }

        /// <summary>The full path the notification is about, as translated via <see cref="SvnTools.GetNormalizedFullPath" /></summary>
        /// <remarks>See also <see cref="Path" />.</remarks>
        public string FullPath
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_fullPath == null && _status != null && _pool != null)
                    _fullPath = SvnBase.Utf8_PathPtrToString(_status.local_abspath, _pool);

                return _fullPath;
            }
        }

        /// <summary>The path returned by the subversion api</summary>
        public string Path
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_path == null && _pPath != null && _pool != null)
                    _path = SvnBase.Utf8_PathPtrToString(_pPath, _pool);

                return _path;
            }
        }

        public string MovedFrom
        {
            get
            {
                if (_movedFrom == null && _status != null && _status.moved_from_abspath != null && _pool != null)
                    _movedFrom = SvnBase.Utf8_PathPtrToString(_status.moved_from_abspath, _pool);

                return _movedFrom;
            }
        }

        public string MovedTo
        {
            get
            {
                if (_movedTo == null && _status != null && _status.moved_to_abspath != null && _pool != null)
                    _movedTo = SvnBase.Utf8_PathPtrToString(_status.moved_to_abspath, _pool);

                return _movedTo;
            }
        }

        public bool Versioned { get; }

        public bool Conflicted { get; }

        public bool Modified
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                switch (LocalNodeStatus)
                {
                    case SvnStatus.Modified:
                    case SvnStatus.Added:
                    case SvnStatus.Deleted:
                    case SvnStatus.Replaced:
                    case SvnStatus.Merged:
                    case SvnStatus.Conflicted:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>The node status (combination of restructuring operations, text and property status.</summary>
        public SvnStatus LocalNodeStatus { get; }

        /// <summary>Content status in working copy</summary>
        public SvnStatus LocalContentStatus
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (LocalNodeStatus != SvnStatus.Modified && LocalNodeStatus != SvnStatus.Conflicted)
                    return LocalNodeStatus;
                else
                    return LocalTextStatus;
            }
        }

        /// <summary>The status of the text/content of the node</summary>
        public SvnStatus LocalTextStatus { get; }

        /// <summary>Property status in working copy</summary>
        public SvnStatus LocalPropertyStatus { get; }

        /// <summary>Gets a boolean indicating whether the workingcopy is locked</summary>
        [Obsolete("Please use .Wedged")]
        public bool LocalLocked => Wedged;

        /// <summary>Gets a boolean indicating whether the workingcopy is locked</summary>
        public bool Wedged { get; }

        /// <summary>Gets a boolean indicating whether the file is copied in the working copy</summary>
        /// <remarks>A file or directory can be 'copied' if it's scheduled for addition-with-history
        /// (or part of a subtree that is scheduled as such.).</remarks>
        public bool LocalCopied { get; }

        public long Revision { get; }

        public long LastChangeRevision { get; }

        public DateTime LastChangeTime { get; }

        public string LastChangeAuthor
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_lastChangeAuthor == null && _status != null && _status.changed_author != null)
                    _lastChangeAuthor = SvnBase.Utf8_PtrToString(_status.changed_author);

                return _lastChangeAuthor;
            }
        }

        public Uri RepositoryRoot
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_reposRoot == null && _status != null && _status.repos_root_url != null)
                    _reposRoot = SvnBase.Utf8_PtrToUri(_status.repos_root_url, SvnNodeKind.Directory);

                return _reposRoot;
            }
        }

        /// <summary>Gets the repository id as Guid</summary>
        public Guid RepositoryId => RepositoryIdValue != null ? new Guid(RepositoryIdValue) : Guid.Empty;

        /// <summary>Gets the repository id as String</summary>
        public string RepositoryIdValue
        {
            get
            {
                if (_repositoryId == null && _status != null && _status.repos_uuid != null)
                    _repositoryId = SvnBase.Utf8_PtrToString(_status.repos_uuid);

                return _repositoryId;
            }
        }

        public Uri Uri
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_uri == null && _status != null && _status.repos_relpath != null && _pool != null)
                {               
                    _uri = SvnBase.Utf8_PtrToUri(
                        svn_path.svn_path_url_add_component2(_status.repos_root_url, _status.repos_relpath, _pool.Handle),
                        NodeKind);
                }
                return _uri;
            }
        }

        /// <summary>Gets a boolean indicating whether the file is switched in the working copy</summary>
        public bool Switched { get; }

        /// <summary>Gets a boolean indicating whether the node is a file external</summary>
        public bool IsFileExternal { get; }

        public SvnLockInfo LocalLock
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_localLock == null && _status != null && _status.@lock != null)
                    _localLock = new SvnLockInfo(_status.@lock, false);

                return _localLock;
            }
        }

        public string ChangeList
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_changelist == null && _status != null && _status.changelist != null)
                    _changelist = SvnBase.Utf8_PtrToString(_status.changelist);

                return _changelist;
            }
        }

        public SvnDepth Depth { get; }

        /// <summary>Gets the out of date status of the item; if true the RemoteUpdate* properties are set</summary>
        public bool IsRemoteUpdated => RemoteUpdateRevision != -1;

        public SvnStatus RemoteContentStatus
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (LocalNodeStatus != SvnStatus.Modified && LocalNodeStatus != SvnStatus.Conflicted)
                    return RemoteNodeStatus;
                else
                    return RemoteTextStatus;
            }
        }

        public SvnStatus RemoteNodeStatus { get; }

        public SvnStatus RemoteTextStatus { get; }

        public SvnStatus RemotePropertyStatus { get; }

        public SvnLockInfo RemoteLock
        {
            get
            {
                if (_reposLock == null && _status != null && _status.repos_lock != null)
                    _reposLock = new SvnLockInfo(_status.repos_lock, false);

                return _reposLock;
            }
        }

        /// <summary>Out of Date: Last commit version of the item</summary>
        public long RemoteUpdateRevision { get; }

        /// <summary>Out of Date: Last commit date of the item</summary>
        public DateTime RemoteUpdateCommitTime { get; }

        /// <summary>Out of Date: Gets the author of the OutOfDate commit</summary>
        public string RemoteUpdateCommitAuthor
        {
            get
            {
                if (_oodLastCommitAuthor == null && _status != null && _status.ood_changed_author != null && IsRemoteUpdated)
                    _oodLastCommitAuthor = SvnBase.Utf8_PtrToString(_status.ood_changed_author);

                return _oodLastCommitAuthor;
            }
        }

        /// <summary>Out of Date: Gets the node kind of the OutOfDate commit</summary>
        public SvnNodeKind RemoteUpdateNodeKind { get; }

        /// <summary>Gets an object containing detailed workingcopy information of this node</summary>
        public SvnWorkingCopyInfo WorkingCopyInfo
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (_wcInfo == null && _status != null && _status.versioned && _client != null && _pool != null)
                    _wcInfo = new SvnWorkingCopyInfo(_status, _client, _pool);

                return _wcInfo;
            }
        }

        /// <summary>The length of the file currently in the working copy, matching the name of this node. -1 if there is no such file.</summary>
        public long FileLength { get; }

        // /// <summary>Gets the tree conflict data of this node or <c>null</c> if this node doesn't have a tree conflict</summary>
        // [Obsolete("Always returns NULL now; use .IsConflicted and a separate call to SvnClient.Info() to retrieve details")]
        // public SvnConflictData TreeConflict => null;


        /// <summary>Gets the raw content status of the node when available</summary>
        [Obsolete("Use .LocalTextStatus")]
        public SvnStatus PristineContentStatus => LocalTextStatus;

        /// <summary>Gets the raw property status of the node when available</summary>
        [Obsolete("Use .LocalPropertyStatus")]
        public SvnStatus PristinePropertyStatus => LocalPropertyStatus;

        protected internal override void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    // Use all properties to get them cached in .Net memory
                    GC.KeepAlive(FullPath);
                    GC.KeepAlive(Path);
                    GC.KeepAlive(LastChangeAuthor);
                    GC.KeepAlive(RepositoryRoot);
                    GC.KeepAlive(RepositoryIdValue);
                    GC.KeepAlive(Uri);
                    GC.KeepAlive(LocalLock);
                    GC.KeepAlive(ChangeList);

                    GC.KeepAlive(RemoteLock);
                    GC.KeepAlive(RemoteUpdateCommitAuthor);
                    GC.KeepAlive(WorkingCopyInfo);
                    GC.KeepAlive(MovedFrom);
                    GC.KeepAlive(MovedTo);
                }

                if (_localLock != null)
                    _localLock.Detach(keepProperties);
                if (_reposLock != null)
                    _reposLock.Detach(keepProperties);
                if (_wcInfo != null)
                    _wcInfo.Detach(keepProperties);
            }
            finally
            {
                _pPath = null;
                _status = null;
                _pool = null;
                _client = null;
                base.Detach(keepProperties);
            }
        }
    }
}
