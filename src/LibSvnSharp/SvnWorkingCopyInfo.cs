using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnWorkingCopyInfo
    {
        // This class looks remarkibly simalar to SvnWorkingCopyEntryEventArgs
        // I don't use them as the same to keep both open for future extensions
        // in different directions
        SvnClientContext _client;
        svn_client_status_t _status;
        svn_wc_entry_t _entry;
        AprPool _pool;
        bool _ensured;

        string _name;
        long _revision;
        Uri _uri;
        Uri _repositoryUri;
        string _repositoryId;
        SvnNodeKind _nodeKind;
        SvnSchedule _schedule;
        bool _copied;
        bool _deleted;
        bool _absent;
        bool _incomplete;
        Uri _copyFrom;
        long _copyFromRev;

        string _conflictOld;
        string _conflictNew;
        string _conflictWork;
        string _prejfile;
        DateTime _textTime;
        string _checksum;
        long _lastChangeRev;
        DateTime _lastChangeTime;
        string _lastChangeAuthor;
        string _lockToken;
        string _lockOwner;
        string _lockComment;
        DateTime _lockTime;
        bool _hasProperties;
        bool _hasPropertyChanges;
        string _changelist;
        long _wcSize;
        bool _keepLocal;
        SvnDepth _depth;
        string _fileExternalPath;
        SvnRevision _fileExternalRevision;
        SvnRevision _fileExternalPegRevision;

        internal SvnWorkingCopyInfo(svn_client_status_t status, SvnClientContext client, AprPool pool)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));
            else if (client == null)
                throw new ArgumentNullException(nameof(client));
            else if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _client = client;
            _status = status;
            _pool = pool;
        }

        internal unsafe void Ensure()
        {
            if (_ensured || _status == null)
                return;

            _ensured = true;

            svn_wc_status2_t.__Internal* status2;

            var error = libsvnsharp_wc_private.svn_wc__status2_from_3(
                (void**) &status2,
                svn_wc_status3_t.__CreateInstance(_status.backwards_compatibility_baton),
                _client.CtxHandle.wc_ctx,
                _status.local_abspath,
                _pool.Handle,
                _pool.Handle);

            if (error != null)
                throw SvnException.Create(error);

            var entry = svn_wc_entry_t.__CreateInstance(status2->entry);
            _entry = entry;

            _revision = entry.revision;
            _nodeKind = (SvnNodeKind)entry.kind;
            _schedule = (SvnSchedule)entry.schedule;
            _copied = entry.copied;
            _deleted = entry.deleted;
            _absent = entry.absent;
            _incomplete = entry.incomplete;
            _copyFromRev = entry.copyfrom_rev;
            _textTime = SvnBase.DateTimeFromAprTime(entry.text_time);
            _lastChangeRev = entry.cmt_rev;
            _lastChangeTime = SvnBase.DateTimeFromAprTime(entry.cmt_date);
            _lockTime = SvnBase.DateTimeFromAprTime(entry.lock_creation_date);
            _hasProperties = entry.has_props;
            _hasPropertyChanges = entry.has_prop_mods;
            _wcSize = entry.working_size;
            _keepLocal = entry.keep_local;
            _depth = (SvnDepth)entry.depth;
        }
        
        /// <summary>The entries name</summary>
        [Obsolete("Use information from SvnStatusEventArgs.Path to avoid expensive lookup")]
        public unsafe string Name
        {
            get
            {
                Ensure();
                if (_name == null && _entry != null && _entry.name != null && _pool != null)
                    _name = SvnBase.Utf8_PathPtrToString(_entry.name, _pool);

                return _name;
            }
        }

        /// <summary>Base revision</summary>
        [Obsolete("Use SvnStatusEventArgs.Revision to avoid expensive lookup")]
        public long Revision
        {
            get
            {
                Ensure();
                return _revision;
            }
        }

        /// <summary>Url in repository, including final '/' if the entry specifies a directory</summary>
        [Obsolete("Use SvnStatusEventArgs.Uri to avoid expensive lookup")]
        public unsafe Uri Uri
        {
            get
            {
                Ensure();
                if (_uri == null && _entry != null && _entry.url != null)
                    _uri = SvnBase.Utf8_PtrToUri(_entry.url, _nodeKind);

                return _uri;
            }
        }

        /// <summary>The repository Uri including a final '/'</summary>
        [Obsolete("Use SvnStatusEventArgs.RepositoryRoot to avoid expensive lookup")]
        public unsafe Uri RepositoryUri
        {
            get
            {
                Ensure();
                if (_repositoryUri == null && _entry != null && _entry.repos != null)
                    _repositoryUri = SvnBase.Utf8_PtrToUri(_entry.repos, SvnNodeKind.Directory);

                return _repositoryUri;
            }
        }

        /// <summary>Gets the repository id as Guid</summary>
        [Obsolete("Use SvnStatusEventArgs.RepositoryId to avoid expensive lookup")]
        public Guid RepositoryId
        {
            get
            {
                Ensure();
                return RepositoryIdValue != null ? new Guid(RepositoryIdValue) : Guid.Empty;
            }
        }

        /// <summary>Gets the repository id as String</summary>
        [Obsolete("Use SvnStatusEventArgs.RepositoryIdValue to avoid expensive lookup")]
        public unsafe string RepositoryIdValue
        {
            get
            {
                Ensure();
                if (_repositoryId == null && _entry != null && _entry.uuid != null)
                    _repositoryId = SvnBase.Utf8_PtrToString(_entry.uuid);

                return _repositoryId;
            }
        }

        /// <summary>Gets the node kind</summary>
        [Obsolete("Use SvnStatusEventArgs.NodeKind to avoid expensive lookup")]
        public SvnNodeKind NodeKind
        {
            get
            {
                Ensure();
                return _nodeKind;
            }
        }

        /// <summary>Gets the node scheduling (add, delete, replace)</summary>
        [Obsolete("Use SvnStatusEventArgs.NodeStatus to avoid expensive lookup")]
        public SvnSchedule Schedule
        {
            get
            {
                Ensure();
                return _schedule;
            }
        }

        /// <summary>Gets a boolean indicating whether the node is in a copied state
        /// (possibly because the entry is a child of a path that is
        /// scheduled for addition or replacement when the entry itself is
        /// normal</summary>
        [Obsolete("Use SvnStatusEventArgs.LocalCopied to avoid expensive lookup")]
        public bool IsCopy
        {
            get
            {
                Ensure();
                return _copied;
            }
        }

        public bool IsDeleted
        {
            get
            {
                Ensure();
                return _deleted;
            }
        }

        public bool IsAbsent
        {
            get
            {
                Ensure();
                return _absent;
            }
        }

        public bool IsIncomplete
        {
            get
            {
                Ensure();
                return _incomplete;
            }
        }

        public unsafe Uri CopiedFrom
        {
            get
            {
                Ensure();
                if (_copyFrom == null && _entry != null && _entry.copyfrom_url != null)
                    _copyFrom = SvnBase.Utf8_PtrToUri(_entry.copyfrom_url, _nodeKind);

                return _copyFrom;
            }
        }

        public long CopiedFromRevision
        {
            get
            {
                Ensure();
                return _copyFromRev;
            }
        }

        public unsafe string ConflictOldFile
        {
            get
            {
                Ensure();
                if (_conflictOld == null && _entry != null && _entry.conflict_old != null && _pool != null)
                    _conflictOld = SvnBase.Utf8_PathPtrToString(_entry.conflict_old, _pool);

                return _conflictOld;
            }
        }

        public unsafe string ConflictNewFile
        {
            get
            {
                Ensure();
                if (_conflictNew == null && _entry != null && _entry.conflict_new != null && _pool != null)
                    _conflictNew = SvnBase.Utf8_PathPtrToString(_entry.conflict_new, _pool);

                return _conflictNew;
            }
        }

        public unsafe string ConflictWorkFile
        {
            get
            {
                Ensure();
                if (_conflictWork == null && _entry != null && _entry.conflict_wrk != null && _pool != null)
                    _conflictWork = SvnBase.Utf8_PathPtrToString(_entry.conflict_wrk, _pool);

                return _conflictWork;
            }
        }

        public unsafe string PropertyRejectFile
        {
            get
            {
                Ensure();
                if (_prejfile == null && _entry != null && _entry.prejfile != null && _pool != null)
                    _prejfile = SvnBase.Utf8_PathPtrToString(_entry.prejfile, _pool);

                return _prejfile;
            }
        }

        [Obsolete("Not used since Subversion 1.4. Always DateTime.MinValue")]
        public DateTime PropertyChangeTime => DateTime.MinValue;

        public DateTime ContentChangeTime
        {
            get
            {
                Ensure();
                return _textTime;
            }
        }

        public unsafe string Checksum
        {
            get
            {
                Ensure();
                if (_checksum == null && _entry != null && _entry.checksum != null)
                    _checksum = SvnBase.Utf8_PtrToString(_entry.checksum);

                return _checksum;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LastChangeRevision to avoid expensive lookup")]
        public long LastChangeRevision
        {
            get
            {
                Ensure();
                return _lastChangeRev;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LastChangeTime to avoid expensive lookup")]
        public DateTime LastChangeTime
        {
            get
            {
                Ensure();
                return _lastChangeTime;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LastChangeAuthor to avoid expensive lookup")]
        public unsafe string LastChangeAuthor
        {
            get
            {
                Ensure();
                if (_lastChangeAuthor == null && _entry != null && _entry.cmt_author != null)
                    _lastChangeAuthor = SvnBase.Utf8_PtrToString(_entry.cmt_author);

                return _lastChangeAuthor;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LocalLock to avoid expensive lookup")]
        public unsafe string LockToken
        {
            get
            {
                Ensure();
                if (_lockToken == null && _entry != null && _entry.lock_token != null)
                    _lockToken = SvnBase.Utf8_PtrToString(_entry.lock_token);

                return _lockToken;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LocalLock to avoid expensive lookup")]
        public unsafe string LockOwner
        {
            get
            {
                Ensure();
                if (_lockOwner == null && _entry != null && _entry.lock_owner != null)
                    _lockOwner = SvnBase.Utf8_PtrToString(_entry.lock_owner);

                return _lockOwner;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LocalLock to avoid expensive lookup")]
        public unsafe string LockComment
        {
            get
            {
                Ensure();
                if (_lockComment == null && _entry != null && _entry.lock_comment != null)
                {
                    _lockComment = SvnBase.Utf8_PtrToString(_entry.lock_comment);

                    if (_lockComment != null)
                        _lockComment = _lockComment.Replace("\n", Environment.NewLine);
                }

                return _lockComment;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LocalLock to avoid expensive lookup")]
        public DateTime LockTime
        {
            get
            {
                Ensure();
                return _lockTime;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LocalPropertyStatus to avoid expensive lookup")]
        public bool HasProperties
        {
            get
            {
                Ensure();
                return _hasProperties;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.LocalPropertyStatus to avoid expensive lookup")]
        public bool HasPropertyChanges
        {
            get
            {
                Ensure();
                return _hasPropertyChanges;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.ChangeList to avoid expensive lookup")]
        public unsafe string ChangeList
        {
            get
            {
                Ensure();
                if (_changelist == null && _entry != null && _entry.changelist != null)
                    _changelist = SvnBase.Utf8_PtrToString(_entry.changelist);

                return _changelist;
            }
        }

        [Obsolete("Use SvntatusEventArgs.FileLength to avoid expensive lookup")]
        public long WorkingCopySize
        {
            get
            {
                Ensure();
                return _wcSize;
            }
        }

        [Obsolete("Unused concept since Subversion 1.7")]
        public bool KeepLocal
        {
            get
            {
                Ensure();
                return _keepLocal;
            }
        }

        [Obsolete("Use SvnStatusEventArgs.Depth to avoid expensive lookup")]
        public SvnDepth Depth
        {
            get
            {
                Ensure();
                return _depth;
            }
        }

        /// <summary>The entry is a intra-repository file external and this is the
        /// repository root relative path to the file specified in the
        /// externals definition, otherwise NULL if the entry is not a file
        /// external.</summary>
        public unsafe string FileExternalPath
        {
            get
            {
                Ensure();
                if (_fileExternalPath == null && _entry != null && _entry.file_external_path != null)
                    _fileExternalPath = SvnBase.Utf8_PtrToString(_entry.file_external_path);

                return _fileExternalPath;
            }
        }

        public unsafe SvnRevision FileExternalRevision
        {
            get
            {
                Ensure();
                if (_fileExternalRevision == null && _entry != null && _entry.file_external_path != null)
                    _fileExternalRevision = SvnRevision.Load(_entry.file_external_rev);

                return _fileExternalRevision;
            }
        }

        public unsafe SvnRevision FileExternalOperationalRevision
        {
            get
            {
                Ensure();
                if (_fileExternalPegRevision == null && _entry != null && _entry.file_external_path != null)
                    _fileExternalPegRevision = SvnRevision.Load(_entry.file_external_peg_rev);

                return _fileExternalPegRevision;
            }
        }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return SvnEventArgs.SafeGetHashCode(Name) ^ SvnEventArgs.SafeGetHashCode(Uri);
        }

        public void Detach()
        {
            Detach(true);
        }

        internal void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    GC.KeepAlive(Name);
                    GC.KeepAlive(Uri);
                    GC.KeepAlive(RepositoryUri);
                    GC.KeepAlive(RepositoryId);
                    GC.KeepAlive(CopiedFrom);
                    GC.KeepAlive(ConflictOldFile);
                    GC.KeepAlive(ConflictNewFile);
                    GC.KeepAlive(ConflictWorkFile);
                    GC.KeepAlive(PropertyRejectFile);
                    GC.KeepAlive(Checksum);
                    GC.KeepAlive(LastChangeAuthor);
                    GC.KeepAlive(LockToken);
                    GC.KeepAlive(LockOwner);
                    GC.KeepAlive(LockComment);
                    GC.KeepAlive(ChangeList);
                    GC.KeepAlive(FileExternalPath);
                    GC.KeepAlive(FileExternalRevision);
                    GC.KeepAlive(FileExternalOperationalRevision);
                }
            }
            finally
            {
                _client = null;
                _status = null;
                _entry = null;
                _pool = null;
            }
        }
    }
}
