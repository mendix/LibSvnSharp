using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Remote;

namespace LibSvnSharp
{
    public class SvnListEventArgs : SvnCancelEventArgs, ISvnRepositoryListItem
    {
        unsafe sbyte* _pAbsPath;
        svn_lock_t _pLock;
        svn_dirent_t _pDirEnt;
        unsafe sbyte* _external_parent_url;
        unsafe sbyte* _external_target;

        string _absPath;
        SvnLockInfo _lock;
        SvnDirEntry _entry;
        Uri _baseUri;
        Uri _entryUri;
        string _name;
        Uri _externalParent;
        string _externalTarget;

        internal unsafe SvnListEventArgs(
            sbyte* path,
            svn_dirent_t dirent,
            svn_lock_t @lock,
            sbyte* absPath,
            Uri repositoryRoot,
            sbyte* externalParentUrl,
            sbyte* externalTarget)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (absPath == null)
                throw new ArgumentNullException(nameof(absPath));

            Path = SvnBase.Utf8_PtrToString(path);

            _pDirEnt = dirent;
            _pLock = @lock;
            _pAbsPath = absPath;
            RepositoryRoot = repositoryRoot;
            _external_parent_url = externalParentUrl;
            _external_target = externalTarget;
        }


        /// <summary>Gets the path of the item</summary>
        public string Path { get; }

        /// <summary>Gets the origin path of the item</summary>
        public unsafe string BasePath
        {
            get
            {
                if (_absPath == null && _pAbsPath != null)
                    _absPath = SvnBase.Utf8_PtrToString(_pAbsPath);

                return _absPath;
            }
        }

        /// <summary>Gets the filename of the item</summary>
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    if (!string.IsNullOrEmpty(Path))
                        _name = System.IO.Path.GetFileName(Path);
                    else if (!string.IsNullOrEmpty(BasePath))
                        _name = System.IO.Path.GetFileName(BasePath);
                    else
                        _name = "";
                }
                return _name;
            }
        }

        /// <summary>When retrieving a listing using an Uri target: contains the repository root</summary>
        /// <value>The Repository root or <c>null</c> when the repository root is not available</value>
        public Uri RepositoryRoot { get; }

        /// <summary>When retrieving a listing using an Uri target: contains the uri from which Path is relative</summary>
        /// <value>The Base Uri or <c>null</c> when the repository root is not available</value>
        public Uri BaseUri
        {
            get
            {
                if (_baseUri == null && (RepositoryRoot != null && BasePath != null))
                {
                    if (string.IsNullOrEmpty(BasePath) ||
                        (BasePath.Length == 1 && BasePath[0] == '/'))
                    {
                        _baseUri = RepositoryRoot;
                    }
                    else
                    {
                        bool isFile = false;
                        if (string.IsNullOrEmpty(Path)) // = Request Root
                        {
                            if (_pDirEnt != null && _pDirEnt.kind != svn_node_kind_t.svn_node_dir)
                                isFile = true;
                            else if (Entry != null && Entry.NodeKind != SvnNodeKind.Directory)
                                isFile = true;
                        }

                        _baseUri = new Uri(RepositoryRoot, SvnBase.PathToUri(BasePath.Substring(1) + (isFile ? "" : "/")));
                    }
                }

                return _baseUri;
            }
        }

        public Uri Uri
        {
            get
            {
                if (_entryUri == null && BaseUri != null && Path != null && Entry != null)
                {
                    if (Path.Length == 0)
                        _entryUri = BaseUri;
                    else if (Entry.NodeKind == SvnNodeKind.Directory)
                        _entryUri = new Uri(BaseUri, SvnBase.PathToUri(Path + "/"));
                    else
                        _entryUri = new Uri(BaseUri, SvnBase.PathToUri(Path));
                }

                return _entryUri;
            }
        }

        [Obsolete("Use .Uri")]
        public Uri EntryUri => Uri;

        public unsafe Uri ExternalParent
        {
            get
            {
                if (_externalParent == null && _external_parent_url != null)
                    _externalParent = SvnBase.Utf8_PtrToUri(_external_parent_url, SvnNodeKind.Directory);

                return _externalParent;
            }
        }

        public unsafe string ExternalTarget
        {
            get
            {
                if (_externalTarget == null && _external_target != null)
                    _externalTarget = SvnBase.Utf8_PtrToString(_external_target);

                return _externalTarget;
            }
        }

        /// <summary>Gets lock information if RetrieveLocks is set on the args object</summary>
        public SvnLockInfo Lock
        {
            get
            {
                if (_lock == null && _pLock != null)
                    _lock = new SvnLockInfo(_pLock, false);

                return _lock;
            }
        }

        /// <summary>Gets the information specified in RetrieveEntries on the args object</summary>
        public SvnDirEntry Entry
        {
            get
            {
                if (_entry == null && _pDirEnt != null)
                    _entry = new SvnDirEntry(_pDirEnt);

                return _entry;
            }
        }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        protected internal override unsafe void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    GC.KeepAlive(Path);
                    GC.KeepAlive(BasePath);
                    GC.KeepAlive(Lock);
                    GC.KeepAlive(Entry);
                    GC.KeepAlive(ExternalParent);
                    GC.KeepAlive(ExternalTarget);
                }

                _lock?.Detach(keepProperties);
                _entry?.Detach(keepProperties);
            }
            finally
            {
                _pAbsPath = null;
                _pLock = null;
                _pDirEnt = null;
                _external_parent_url = null;
                _external_target = null;

                base.Detach(keepProperties);
            }
        }
    }
}
