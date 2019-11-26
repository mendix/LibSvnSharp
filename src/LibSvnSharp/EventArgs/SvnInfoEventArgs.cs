using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public class SvnInfoEventArgs : SvnCancelEventArgs
    {
        svn_client_info2_t _info;
        AprPool _pool;

        string _fullPath;
        Uri _uri;
        Uri _reposRootUri;
        string _reposUuid;
        string _lastChangeAuthor;
        SvnLockInfo _lock;
        SvnSchedule _wcSchedule;
        Uri _copyFromUri;
        string _checksum;
        string _changelist;
        ICollection<SvnConflictData> _conflicts;
        string _wcAbspath;
        string _movedFromAbspath;
        string _movedToAbspath;

        internal SvnInfoEventArgs(string path, svn_client_info2_t info, AprPool pool)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _info = info;
            _pool = pool;

            Path = path;
            Revision = info.rev;
            NodeKind = (SvnNodeKind)info.kind;
            LastChangeRevision = info.last_changed_rev;
            LastChangeTime = SvnBase.DateTimeFromAprTime(info.last_changed_date);
            HasLocalInfo = (info.wc_info != null);

            if (info.wc_info != null)
            {
                _wcSchedule = (SvnSchedule)info.wc_info.schedule;
                CopyFromRevision = info.wc_info.copyfrom_rev;
                Depth = (SvnDepth)info.wc_info.depth;

                ContentTime = SvnBase.DateTimeFromAprTime(info.wc_info.recorded_time);
                if (info.wc_info.recorded_size == -1)
                    WorkingCopySize = -1;
                else
                    WorkingCopySize = info.wc_info.recorded_size;

                Conflicted = info.wc_info.conflicts != null && (info.wc_info.conflicts.nelts > 0);
            }
            else
            {
                Depth = SvnDepth.Unknown;
                WorkingCopySize = -1;
                CopyFromRevision = -1;
            }

            if (info.size == -1)
                RepositorySize = -1;
            else
                RepositorySize = info.size;
        }

        /// <summary>Gets the path of the file. The local path if requisting WORKING version, otherwise the name of the file
        /// at the specified version</summary>
        public string Path { get; }

        /// <summary>The path the notification is about, translated via <see cref="SvnTools.GetNormalizedFullPath" /></summary>
        /// <remarks>The <see cref="FullPath" /> property contains the path in normalized format; while <see cref="Path" /> returns the exact path from the subversion api</remarks>
        public string FullPath
        {
            get
            {
                if (_fullPath == null && Path != null && HasLocalInfo)
                    _fullPath = SvnTools.GetNormalizedFullPath(Path);

                return _fullPath;
            }
        }

        /// <summary>Gets the full Uri of the node</summary>
        public unsafe Uri Uri
        {
            get
            {
                if (_uri == null && _info != null && _info.URL != null)
                    _uri = SvnBase.Utf8_PtrToUri(_info.URL, NodeKind);

                return _uri;
            }
        }

        /// <summary>Gets the queried revision</summary>
        public long Revision { get; }

        /// <summary>Gets the node kind of the specified node</summary>
        public SvnNodeKind NodeKind { get; }

        /// <summary>Gets the repository root Uri; ending in a '/'</summary>
        /// <remarks>The unmanaged api does not add the '/' at the end, but this makes using <see cref="System.Uri" /> hard</remarks>
        public unsafe Uri RepositoryRoot
        {
            get
            {
                if (_reposRootUri == null && _info != null && _info.repos_root_URL != null)
                    _reposRootUri = SvnBase.Utf8_PtrToUri(_info.repos_root_URL, SvnNodeKind.Directory);

                return _reposRootUri;
            }
        }

        /// <summary>Gets the uuid of the repository (if available). Otherwise Guid.Empty</summary>
        public Guid RepositoryId => RepositoryIdValue != null ? new Guid(RepositoryIdValue) : Guid.Empty;

        /// <summary>Gets the repository uuid or <c>null</c> if not available</summary>
        public unsafe string RepositoryIdValue
        {
            get
            {
                if (_reposUuid == null && _info != null && _info.repos_UUID != null)
                    _reposUuid = SvnBase.Utf8_PtrToString(_info.repos_UUID);

                return _reposUuid;
            }
        }

        /// <summary>Gets the last revision in which node (or one of its descendants) changed</summary>
        public long LastChangeRevision { get; }

        /// <summary>Gets the timestamp of the last revision in which node (or one of its descendants) changed</summary>
        public DateTime LastChangeTime { get; }

        /// <summary>Gets the author of the last revision in which node (or one of its descendants) changed</summary>
        public unsafe string LastChangeAuthor
        {
            get
            {
                if (_lastChangeAuthor == null && _info != null && _info.last_changed_author != null)
                    _lastChangeAuthor = SvnBase.Utf8_PtrToString(_info.last_changed_author);

                return _lastChangeAuthor;
            }
        }

        /// <summary>Gets information about the current lock on node</summary>
        public unsafe SvnLockInfo Lock
        {
            get
            {
                if (_lock == null && _info?.@lock != null && _info.@lock.token != null)
                    _lock = new SvnLockInfo(_info.@lock, HasLocalInfo);

                return _lock;
            }
        }
        
        /// <summary>Gets a boolean indicating whether working copy information is available in this instance</summary>
        public bool HasLocalInfo { get; }

        public SvnSchedule Schedule => HasLocalInfo ? _wcSchedule : SvnSchedule.Normal;

        public unsafe Uri CopyFromUri
        {
            get
            {
                if (_copyFromUri == null && _info?.wc_info != null && _info.wc_info.copyfrom_url != null)
                    _copyFromUri = SvnBase.Utf8_PtrToUri(_info.wc_info.copyfrom_url, NodeKind);

                return _copyFromUri;
            }
        }

        public long CopyFromRevision { get; }

        [Obsolete("Please use .CopyFromRevision")]
        public long CopyFromRev => CopyFromRevision;

        public DateTime ContentTime { get; }

        [Obsolete("Always returns (and always returned) DateTime.MinValue")]
        public DateTime PropertyTime => DateTime.MinValue;

        /// <summary>The SHA1 checksum of the file. (Used to return a MD5 checksom in Subversion &lt;= 1.6)</summary>
        public unsafe string Checksum
        {
            get
            {
                if (_checksum == null && _info?.wc_info?.checksum != null && _pool != null)
                    _checksum = SvnBase.Utf8_PtrToString(svn_checksum.svn_checksum_to_cstring(_info.wc_info.checksum, _pool.Handle));

                return _checksum;
            }
        }

        public unsafe ICollection<SvnConflictData> Conflicts
        {
            get
            {
                if (!Conflicted || _conflicts != null || _info == null || _info.wc_info == null)
                    return _conflicts;

                var items = new List<SvnConflictData>();

                for (int i = 0; i < _info.wc_info.conflicts.nelts; i++)
                {
                    var c_ptr = ((svn_wc_conflict_description2_t.__Internal**) _info.wc_info.conflicts.elts)[i];
                    var c = svn_wc_conflict_description2_t.__CreateInstance(new IntPtr(c_ptr));

                    items.Add(new SvnConflictData(c, _pool));
                }

                return _conflicts = items.AsReadOnly();
            }
        }

        public string ConflictOld
        {
            get
            {
                if (Conflicted && Conflicts != null)
                {               
                    foreach (SvnConflictData d in Conflicts)
                    {
                        if (d.ConflictType == SvnConflictType.Content)
                            return d.BaseFile;
                    }
                }
                
                return null;
            }
        }

        public string ConflictNew
        {
            get
            {
                if (Conflicted && Conflicts != null)
                {               
                    foreach (SvnConflictData d in Conflicts)
                    {
                        if (d.ConflictType == SvnConflictType.Content)
                            return d.TheirFile;
                    }
                }

                return null;
            }
        }

        public string ConflictWork
        {
            get
            {
                if (Conflicted && Conflicts != null)
                {               
                    foreach (SvnConflictData d in Conflicts)
                    {
                        if (d.ConflictType == SvnConflictType.Content)
                            return d.MyFile;
                    }
                }

                return null;
            }
        }

        public string PropertyEditFile
        {
            get
            {
                if (Conflicted && Conflicts != null)
                {               
                    foreach (SvnConflictData d in Conflicts)
                    {
                        if (d.ConflictType == SvnConflictType.Property)
                            return d.TheirFile;
                    }
                }

                return null;
            }
        }

        public SvnDepth Depth { get; }

        public unsafe string ChangeList
        {
            get
            {
                if (_changelist == null && _info?.wc_info != null && _info.wc_info.changelist != null)
                    _changelist = SvnBase.Utf8_PtrToString(_info.wc_info.changelist);

                return _changelist;
            }
        }

        public long WorkingCopySize { get; }

        public long RepositorySize { get; }

        public SvnConflictData TreeConflict
        {
            get
            {
                if (Conflicted && Conflicts != null)
                {               
                    foreach (SvnConflictData d in Conflicts)
                    {
                        if (d.ConflictType == SvnConflictType.Tree)
                            return d;
                    }
                }

                return null;
            }
        }

        public bool Conflicted { get; }

        public unsafe string WorkingCopyRoot
        {
            get
            {
                if (_wcAbspath == null && _info?.wc_info != null && _pool != null)
                    _wcAbspath = SvnBase.Utf8_PathPtrToString(_info.wc_info.wcroot_abspath, _pool);

                return _wcAbspath;
            }
        }

        public unsafe string MovedFrom
        {
            get
            {
                if (_movedFromAbspath == null && _info?.wc_info != null && _info.wc_info.moved_from_abspath != null)
                    _movedFromAbspath = SvnBase.Utf8_PathPtrToString(_info.wc_info.moved_from_abspath, _pool);

                return _movedFromAbspath;
            }
        }

        public unsafe string MovedTo
        {
            get
            {
                if (_movedToAbspath == null && _info?.wc_info != null && _info.wc_info.moved_to_abspath != null)
                    _movedToAbspath = SvnBase.Utf8_PathPtrToString(_info.wc_info.moved_to_abspath, _pool);

                return _movedToAbspath;
            }
        }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return SafeGetHashCode(Path) ^ SafeGetHashCode(Uri);
        }

        protected internal override void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    // Use all properties to get them cached in .Net memory
                    GC.KeepAlive(Path);
                    GC.KeepAlive(Uri);
                    GC.KeepAlive(RepositoryRoot);
                    GC.KeepAlive(RepositoryId);
                    GC.KeepAlive(LastChangeAuthor);
                    GC.KeepAlive(Lock);
                    GC.KeepAlive(CopyFromUri);
                    GC.KeepAlive(Checksum);
                    GC.KeepAlive(ConflictOld);
                    GC.KeepAlive(ConflictNew);
                    GC.KeepAlive(ConflictWork);
                    GC.KeepAlive(PropertyEditFile);
                    GC.KeepAlive(ChangeList);
                    GC.KeepAlive(Conflicts);
                    GC.KeepAlive(WorkingCopyRoot);
                    GC.KeepAlive(MovedFrom);
                    GC.KeepAlive(MovedTo);
                }

                if (_conflicts != null)
                {               
                    foreach (SvnConflictData cd in _conflicts)
                        cd.Detach(keepProperties);
                }                       

                _lock?.Detach(keepProperties);
            }
            finally
            {
                _info = null;
                _pool = null;
                base.Detach(keepProperties);
            }
        }
    }
}
