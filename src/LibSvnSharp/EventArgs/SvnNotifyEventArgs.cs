using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnNotifyEventArgs : SvnEventArgs
    {
        svn_wc_notify_t _notify;
        AprPool _pool;

        readonly ulong _hunkOriginalStart;
        readonly ulong _hunkOriginalLength;
        readonly ulong _hunkModifiedStart;
        readonly ulong _hunkModifiedLength;
        readonly ulong _hunkMatchedLine;
        readonly ulong _hunkFuzz;

        SvnLockInfo _lock;

        string _changelistName;

        SvnMergeRange _mergeRange;
        bool _pathIsUri;
        bool _mimeTypeIsBinary;

        string _propertyName;
        SvnPropertyCollection _revProps;

        string _fullPath;
        string _path;
        Uri _uri;
        string _mimeType;
        SvnException _exception;

        internal SvnNotifyEventArgs(svn_wc_notify_t notify, SvnCommandType commandType, AprPool pool)
        {
            _notify = notify ?? throw new ArgumentNullException(nameof(notify));
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));

            Action = (SvnNotifyAction) notify.action;
            NodeKind = (SvnNodeKind) notify.kind;
            ContentState = (SvnNotifyState) notify.content_state;
            PropertyState = (SvnNotifyState) notify.prop_state;
            LockState = (SvnLockState) notify.lock_state;
            Revision = notify.revision;
            OldRevision = notify.old_revision;

            _hunkOriginalStart = notify.hunk_original_start;
            _hunkOriginalLength = notify.hunk_original_length;
            _hunkModifiedStart = notify.hunk_modified_start;
            _hunkModifiedLength = notify.hunk_modified_length;
            _hunkMatchedLine = notify.hunk_matched_line;
            _hunkFuzz = notify.hunk_fuzz;

            CommandType = commandType;
        }

        /// <summary>The path the notification is about</summary>
        /// <remarks>The <see cref="FullPath" /> property contains the path in normalized format; while <see cref="Path" /> returns the exact path from the subversion api</remarks>
        public unsafe string Path
        {
            get
            {
                if (_path == null && _notify != null && _notify.path != null && _pool != null &&
                    (_notify.url == null || (_notify.path[0] != '\0' && !(_notify.path[0] == '.' && _notify.path[1] == '\0'))))
                {
                    _path = SvnBase.Utf8_PathPtrToString(_notify.path, _pool);

                    if (svn_dirent_uri.svn_dirent_is_absolute(_notify.path))
                        _fullPath = _path;
                }

                return _path;
            }
        }

        /// <summary>Gets the (relative or absolute uri) Uri the notification is about</summary>
        public unsafe Uri Uri
        {
            get
            {
                if (_uri == null && _notify != null && _notify.url != null)
                    _uri = SvnBase.Utf8_PtrToUri(_notify.url, NodeKind);

                return _uri;
            }
        }

        /// <summary>The path the notification is about, translated via <see cref="SvnTools.GetNormalizedFullPath" /></summary>
        /// <remarks>The <see cref="FullPath" /> property contains the path in normalized format; while <see cref="Path" /> returns the exact path from the subversion api</remarks>
        public string FullPath
        {
            get
            {
                if (_fullPath == null && Path != null)
                {
                    if (_fullPath == null) /* Might be set by .Path */
                        _fullPath = SvnTools.GetNormalizedFullPath(_path);
                }

                return _fullPath;
            }
        }

        /// <summary>Gets a boolean indicating whether the path is a Uri</summary>
        [Obsolete("Check .Uri and (Path == null)")]
        public bool PathIsUri => (Uri != null);

        /// <summary>Gets the commandtype of the command responsible for calling the notify</summary>
        public SvnCommandType CommandType { get; }

        /// <summary>Action that describes what happened to path/url</summary>
        public SvnNotifyAction Action { get; }

        /// <summary>Node kind of path/url</summary>
        public SvnNodeKind NodeKind { get; }

        /// <summary>If non-NULL, indicates the mime-type of @c path. It is always @c NULL for directories.</summary>
        public unsafe string MimeType
        {
            get
            {
                if (_mimeType == null && _notify != null && _notify.mime_type != null)
                {
                    _mimeType = SvnBase.Utf8_PtrToString(_notify.mime_type);
                    _mimeTypeIsBinary = svn_types.svn_mime_type_is_binary(_notify.mime_type);
                }

                return _mimeType;
            }
        }

        /// <summary>If MimeType is not null, a boolean indicating whether this mime type is interpreted as binary</summary>
        public bool MimeTypeIsBinary
        {
            get
            {
                GC.KeepAlive(MimeType);
                return _mimeTypeIsBinary;
            }
        }

        /// <summary>Points to an error describing the reason for the failure when
        /// action is one of the following: @c svn_wc_notify_failed_lock, svn_wc_notify_failed_unlock,
        /// svn_wc_notify_failed_external. Is @c NULL otherwise.</summary>
        public SvnException Error
        {
            get
            {
                if (_exception == null && _notify?.err != null)
                    _exception = (SvnException) SvnException.Create(_notify.err, false);

                return _exception;
            }
        }

        /// <summary>The type of notification that is occurring about node content.</summary>
        public SvnNotifyState ContentState { get; }

        /// <summary>The type of notification that is occurring about node properties.</summary>
        public SvnNotifyState PropertyState { get; }

        /// <summary>Reflects the addition or removal of a lock token in the working copy.</summary>
        public SvnLockState LockState { get; }

        /// <summary>When action is svn_wc_notify_update_completed, target revision
        /// of the update, or @c SVN_INVALID_REVNUM if not available; when action is
        /// c svn_wc_notify_blame_revision, processed revision. In all other cases,
        /// it is @c SVN_INVALID_REVNUM.</summary>
        public long Revision { get; }

        /// <summary>The base revision before updating</summary>
        public long OldRevision { get; }

        /// <summary>Points to the lock structure received from the repository when
        /// action is @c svn_wc_notify_locked.  For other actions, it is
        /// NULL.</summary>
        public SvnLockInfo Lock
        {
            get
            {
                if (_lock == null && _notify?.@lock != null)
                    _lock = new SvnLockInfo(_notify.@lock, false);

                return _lock;
            }
        }

        /// <summary>When action is svn_wc_notify_changelist_add or name.  In all other
        /// cases, it is NULL</summary>
        public unsafe string ChangeListName
        {
            get
            {
                if (_changelistName == null && _notify != null && _notify.changelist_name != null)
                    _changelistName = SvnBase.Utf8_PtrToString(_notify.changelist_name);

                return _changelistName;
            }
        }

        /// <summary>When @c action is @c svn_wc_notify_merge_begin, and both the left and right sides
        /// of the merge are from the same URL.  In all other cases, it is NULL</summary>
        public SvnMergeRange MergeRange
        {
            get
            {
                if (_mergeRange == null && _notify?.merge_range != null)
                    _mergeRange = new SvnMergeRange(_notify.merge_range.start, _notify.merge_range.end, _notify.merge_range.inheritable);

                return _mergeRange;
            }
        }

        /// <summary>If action relates to properties, specifies the name of the property.</summary>
        public unsafe string PropertyName
        {
            get
            {
                if (_propertyName == null && _notify != null && _notify.prop_name != null)
                    _propertyName = SvnBase.Utf8_PtrToString(_notify.prop_name);

                return _propertyName;
            }
        }

        /// <summary>If action is svn_wc_notify_blame_revision, contains a list of revision properties for the specified revision</summary>
        public SvnPropertyCollection RevisionProperties
        {
            get
            {
                if (_revProps == null && _notify?.rev_props != null && _pool != null)
                    _revProps = SvnBase.CreatePropertyDictionary(_notify.rev_props, _pool);

                return _revProps;
            }
        }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return SafeGetHashCode(Path) ^ Revision.GetHashCode() ^ ContentState.GetHashCode();
        }

        /// <summary>Detaches the SvnEventArgs from the unmanaged storage; optionally keeping the property values for later use</summary>
        /// <description>After this method is called all properties are either stored managed, or are no longer readable</description>
        protected internal override void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    // Use all properties to get them cached in .Net memory
                    GC.KeepAlive(Path);
                    GC.KeepAlive(FullPath);
                    GC.KeepAlive(Uri);
                    GC.KeepAlive(MimeType);
                    GC.KeepAlive(Error);
                    GC.KeepAlive(Lock);
                    GC.KeepAlive(ChangeListName);
                    GC.KeepAlive(MergeRange);
                    GC.KeepAlive(PropertyName);
                    GC.KeepAlive(RevisionProperties);
                }

                _lock?.Detach(keepProperties);
            }
            finally
            {
                _notify = null;
                _pool = null;
                base.Detach(keepProperties);
            }
        }
    }
}
