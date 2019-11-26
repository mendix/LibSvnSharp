using System;
using System.IO;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    /// <summary>Subversion client instance; main entrance to the Subversion Client api</summary>
    /// <threadsafety static="true" instance="false"/>
    public partial class SvnClient : SvnClientContext
    {
        readonly AprBaton<SvnClient> _clientBaton;

        SvnClientConfiguration _config;

        ///<summary>Initializes a new <see cref="SvnClient" /> instance with default properties</summary>
        public SvnClient() : base(new AprPool())
        {
            _clientBaton = new AprBaton<SvnClient>(this);
            Initialize();
        }

        void Initialize()
        {
            var baton = _clientBaton.Handle;

            CtxHandle.notify_baton2 = baton;
            CtxHandle.notify_func2 = _callbacks.svn_wc_notify_func2.Get();
            CtxHandle.conflict_baton2 = baton;
            CtxHandle.conflict_func2 = _callbacks.svn_wc_conflict_resolver_func.Get();
        }

        /// <summary>Gets the version number of subversion library</summary>
        public static Version Version
        {
            get
            {
                var version = svn_client.svn_client_version();
                return new Version(version.major, version.minor, version.patch);
            }
        }

        /// <summary>Adds the specified client name to web requests' UserAgent string</summary>
        /// <remarks>The name is filtered to be unique and conformant for the webrequest. Clients should use only alphanumerical ascii characters</remarks>
        public static void AddClientName(string name, Version version)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name, i) && 0 > "._ ".IndexOf(name[i]))
                    throw new ArgumentException(SharpSvnStrings.InvalidCharacterInClientName, nameof(name));
            }

            if (!_clientNames.Contains(name))
            {
                _clientNames.Add(name);
                _clientName += " " + name + "/" + version.ToString();
            }
        }

        /// <summary>Gets the <see cref="SvnClientConfiguration" /> instance of this <see cref="SvnClient"/></summary>
        public SvnClientConfiguration Configuration
        {
            get
            {
                if (_config == null)
                    _config = new SvnClientConfiguration(this);

                return _config;
            }
        }

        /// <summary>
        /// Raised to allow canceling operations. The event is first
        /// raised on the <see cref="SvnClientArgs" /> object and
        /// then on the <see cref="SvnClient" />
        /// </summary>
        public event EventHandler<SvnCancelEventArgs> Cancel;

        /// <summary>
        /// Raised on progress. The event is first
        /// raised on the <see cref="SvnClientArgs" /> object and
        /// then on the <see cref="SvnClient" />
        /// </summary>
        public event EventHandler<SvnProgressEventArgs> Progress;

        /// <summary>
        /// Raised on notifications. The event is first
        /// raised on the <see cref="SvnClientArgs" /> object and
        /// then on the <see cref="SvnClient" />
        /// </summary>
        public event EventHandler<SvnNotifyEventArgs> Notify;

        /// <summary>
        /// Raised on progress. The event is first
        /// raised on the <see cref="SvnClientArgsWithCommit" /> object and
        /// then on the <see cref="SvnClient" />
        /// </summary>
        public event EventHandler<SvnCommittingEventArgs> Committing;

        /// <summary>
        /// Raised on progress. The event is first
        /// raised on the <see cref="SvnClientArgsWithCommit" /> object and
        /// then on the <see cref="SvnClient" />
        /// </summary>
        public event EventHandler<SvnCommittedEventArgs> Committed;

        /// <summary>
        /// Raised on conflict. The event is first
        /// raised on the <see cref="SvnClientArgsWithConflict" /> object and
        /// then on the <see cref="SvnClient" />
        /// </summary>
        public event EventHandler<SvnConflictEventArgs> Conflict;

        /// <summary>
        /// Raised when a subversion exception occurs.
        /// Set <see cref="SvnErrorEventArgs.Cancel" /> to true to cancel
        /// throwing the exception
        /// </summary>
        public event EventHandler<SvnErrorEventArgs> SvnError;

        /// <summary>
        /// Raised just before a command is executed. This allows a listener
        /// to cleanup before a new command is started
        /// </summary>
        public event EventHandler<SvnProcessingEventArgs> Processing;

        /// <summary>Raises the <see cref="Cancel" /> event.</summary>
        protected virtual void OnCancel(SvnCancelEventArgs e)
        {
            Cancel?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Progress" /> event.</summary>
        protected virtual void OnProgress(SvnProgressEventArgs e)
        {
            Progress?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Committing" /> event.</summary>
        protected virtual void OnCommitting(SvnCommittingEventArgs e)
        {
            Committing?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Committed" /> event.</summary>
        protected virtual void OnCommitted(SvnCommittedEventArgs e)
        {
            Committed?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Notify" /> event.</summary>
        protected virtual void OnNotify(SvnNotifyEventArgs e)
        {
            Notify?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Conflict" /> event.</summary>
        protected virtual void OnConflict(SvnConflictEventArgs e)
        {
            Conflict?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Exception" /> event.</summary>
        protected virtual void OnSvnError(SvnErrorEventArgs e)
        {
            SvnError?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Processing" /> event.</summary>
        protected virtual void OnProcessing(SvnProcessingEventArgs e)
        {
            Processing?.Invoke(this, e);
        }

        internal override sealed void HandleClientCancel(SvnCancelEventArgs e)
        {
            base.HandleClientCancel(e);

            if (!e.Cancel)
                OnCancel(e);
        }

        internal override sealed void HandleClientProgress(SvnProgressEventArgs e)
        {
            base.HandleClientProgress(e);

            OnProgress(e);
        }

        internal override sealed void HandleClientCommitting(SvnCommittingEventArgs e)
        {
            base.HandleClientCommitting(e);

            if (!e.Cancel)
                OnCommitting(e);
        }

        internal override sealed void HandleClientCommitted(SvnCommittedEventArgs e)
        {
            base.HandleClientCommitted(e);

            OnCommitted(e);
        }

        internal override sealed void HandleClientNotify(SvnNotifyEventArgs e)
        {
            base.HandleClientNotify(e);

            OnNotify(e);
        }

        internal void HandleClientConflict(SvnConflictEventArgs e)
        {
            if (e.Cancel)
                return;

            if (CurrentCommandArgs is SvnClientArgsWithConflict conflictArgs)
            {
                conflictArgs.RaiseConflict(e);

                if (e.Cancel)
                    return;
            }

            OnConflict(e);
        }

        internal override sealed void HandleClientError(SvnErrorEventArgs e)
        {
            base.HandleClientError(e);

            if (!e.Cancel)
                OnSvnError(e);
        }

        internal override sealed void HandleProcessing(SvnProcessingEventArgs e)
        {
            base.HandleProcessing(e);

            OnProcessing(e);
        }

        /////////////////////////////////////////

        /// <summary>Gets the repository Uri of a path, or <c>null</c> if path is not versioned</summary>
        /// <remarks>See also <see cref="SvnTools.GetUriFromWorkingCopy" /></remarks>
        public unsafe Uri GetUriFromWorkingCopy(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            var pool = new AprPool(_pool);
            var store = new NoArgsStore(this, pool);

            try
            {
                sbyte* url = null;

                svn_error_t err = svn_client.svn_client_url_from_path2(
                    &url,
                    pool.AllocAbsoluteDirent(path),
                    CtxHandle,
                    pool.Handle,
                    pool.Handle);

                if (err == null && url != null)
                    return Utf8_PtrToUri(url, Directory.Exists(path) ? SvnNodeKind.Directory : SvnNodeKind.File);
                if (err != null)
                    svn_error.svn_error_clear(err);

                return null;
            }
            finally
            {
                store.Dispose();
                pool.Dispose();
            }
        }

        /// <summary>Gets the repository root from the specified path</summary>
        /// <value>The repository root <see cref="Uri" /> or <c>null</c> if the uri is not a working copy path</value>
        /// <remarks>LibSvnSharp makes sure the uri ends in a '/'</remarks>
        public Uri GetRepositoryRoot(string target)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException(nameof(target));
            if (!IsNotUri(target))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(target));

            if (TryGetRepository(target, out var reposRoot, out _))
                return reposRoot;

            return null;
        }

        /// <summary>Gets the repository url and repository id for a working copy path</summary>
        public unsafe bool TryGetRepository(string path, out Uri repositoryUrl, out Guid id)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            id = Guid.Empty;
            repositoryUrl = null;

            EnsureState(SvnContextState.ConfigLoaded);

            using (var pool = new AprPool(_pool))
            using (var store = new NoArgsStore(this, pool))
            {
                sbyte* urlStr = null;
                sbyte* uuidStr = null;

                svn_error_t err = svn_client.svn_client_get_repos_root(
                    &urlStr,
                    &uuidStr,
                    pool.AllocAbsoluteDirent(path),
                    CtxHandle,
                    pool.Handle,
                    pool.Handle);

                if (err != null || urlStr == null || uuidStr == null || *urlStr == 0 || *uuidStr == 0)
                {
                    svn_error.svn_error_clear(err);
                    return false;
                }

                repositoryUrl = Utf8_PtrToUri(urlStr, SvnNodeKind.Directory);
                id = new Guid(Utf8_PtrToString(uuidStr));

                return true;
            }
        }

        /// <summary>Gets the (relevant) working copy root of a path or <c>null</c> if the path doesn't have one</summary>
        public unsafe string GetWorkingCopyRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            sbyte* wcroot_abspath = null;
            EnsureState(SvnContextState.ConfigLoaded);
            using (var pool = new AprPool(_pool))
            using (var store = new NoArgsStore(this, pool))
            {
                svn_error_t err = svn_client.svn_client_get_wc_root(
                    &wcroot_abspath,
                    pool.AllocAbsoluteDirent(path),
                    CtxHandle,
                    pool.Handle,
                    pool.Handle);

                if (err == null && wcroot_abspath != null)
                    return Utf8_PathPtrToString(wcroot_abspath, pool);
                if (err != null)
                    svn_error.svn_error_clear(err);

                return null;
            }
        }

        /////////////////////////////////////////

        protected override void DisposeInternal()
        {
            _clientBaton?.Dispose();
        }
    }
}
