using System;
using System.Collections.Generic;
using System.ComponentModel;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;
using LibSvnSharp.Security;

namespace LibSvnSharp
{
    /// <summary>Subversion Client Context wrapper; base class of objects using client context</summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnClientContext : SvnBase, IComponent, IDisposable
    {
        AprBaton<SvnClientContext> _ctxBaton;
        svn_client_ctx_t _ctx;
        internal AprPool _pool;
        int _authCookie;
        SvnContextState _contextState;
        SvnExtendedState _xState;
        readonly SvnAuthentication _authentication;
        SvnClientContext _parent;

        internal bool _noLogMessageRequired;

        // For SvnClient and SvnReposClient
        internal SvnClientArgs _currentArgs;

        // Used config path; used for the authentication cache
        internal string _configPath;

        internal bool _useUserDiff;

        // Holds the handles of long-living callback delegates
        internal readonly SvnClientCallbacks _callbacks;

        List<SvnConfigItem> _configOverrides;

        internal unsafe SvnClientContext(AprPool pool)
        {
            if (pool is null)
                throw new ArgumentNullException(nameof(pool));

            _ctxBaton = new AprBaton<SvnClientContext>(this);
            _callbacks = new SvnClientCallbacks();

            _pool = pool;
            svn_client_ctx_t.__Internal* ctxInternal = null;

            // We manage the config hash ourselves
            var error = svn_client.svn_client_create_context2((void**) &ctxInternal, null, pool.Handle);
            if (error != null)
                throw SvnException.Create(error);

            var ctx = svn_client_ctx_t.__CreateInstance(new IntPtr(ctxInternal));

            ctx.cancel_func = _callbacks.libsvnsharp_cancel_func.Get();
            ctx.cancel_baton = _ctxBaton.Handle;
            ctx.progress_func = _callbacks.libsvnsharp_progress_func.Get();
            ctx.progress_baton = _ctxBaton.Handle;
            ctx.log_msg_func3 = _callbacks.libsvnsharp_commit_log_func.Get();
            ctx.log_msg_baton3 = _ctxBaton.Handle;

            //TODO:
            //ctx.check_tunnel_func = libsvnsharp_check_tunnel_func;
            //ctx.open_tunnel_func = libsvnsharp_open_tunnel_func;
            //ctx.tunnel_baton = _ctxBaton.Handle;

            ctx.client_name = pool.AllocString(SvnBase._clientName);

            _ctx = ctx;
            _authentication = new SvnAuthentication(this, pool);
        }

        internal SvnClientContext(AprPool pool, SvnClientContext client)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            _ctxBaton = new AprBaton<SvnClientContext>(this);

            _pool = pool;
            _parent = client;

            _ctx = client.CtxHandle;
            _authentication = client.Authentication;
        }

        internal SvnClientArgs CurrentCommandArgs => _currentArgs;

        internal virtual void HandleClientCancel(SvnCancelEventArgs e)
        {
            CurrentCommandArgs?.RaiseOnCancel(e);
        }

        internal virtual void HandleClientError(SvnErrorEventArgs e)
        {
            CurrentCommandArgs?.RaiseOnSvnError(e);
        }

        internal virtual void HandleClientProgress(SvnProgressEventArgs e)
        {
            CurrentCommandArgs?.RaiseOnProgress(e);
        }

        internal virtual void HandleProcessing(SvnProcessingEventArgs e)
        {
        }

        internal virtual void HandleClientCommitting(SvnCommittingEventArgs e)
        {
            if (CurrentCommandArgs is SvnClientArgsWithCommit commitArgs)
                commitArgs.RaiseOnCommitting(e);
        }

        internal virtual void HandleClientCommitted(SvnCommittedEventArgs e)
        {
            if (CurrentCommandArgs is SvnClientArgsWithCommit commitArgs)
                commitArgs.RaiseOnCommitted(e);
        }

        internal virtual void HandleClientNotify(SvnNotifyEventArgs e)
        {
            CurrentCommandArgs?.RaiseOnNotify(e);
        }

        public bool IsCommandRunning => (_currentArgs != null);

        public bool IsDisposed => _ctx is null;

        public bool KeepSession { get; set; }

        public ISite Site { get; set; }

        event EventHandler IComponent.Disposed
        {
            add => throw new NotImplementedException();
            remove => throw new NotImplementedException();
        }

        public void Dispose()
        {
            DisposeInternal();
            _ctxBaton.Dispose();
            _ctx = null;
            _pool = null;
            _parent = null;
            _callbacks.Dispose();
        }

        protected virtual void DisposeInternal() { }

        internal svn_client_ctx_t CtxHandle
        {
            get
            {
                if (_ctx is null)
                    throw new ObjectDisposedException("SvnClientContext");

                _pool.Ensure();

                return _ctx;
            }
        }

        internal AprPool ContextPool => _pool;

        internal void SetConfigurationOption(string file, string section, string option, string value)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (_parent != null || State > SvnContextState.ConfigLoaded)
                throw new InvalidOperationException();

            if (!string.Equals(file, SvnConfigNames.ConfigCategory)
                && !string.Equals(file, SvnConfigNames.ServersCategory))
            {
                throw new ArgumentOutOfRangeException(nameof(file));
            }

            if (_configOverrides == null)
                _configOverrides = new List<SvnConfigItem>();

            _configOverrides.Add(new SvnConfigItem(file, section, option, value));
        }

        unsafe void ApplyMimeTypes()
        {
            if (_parent != null)
                throw new InvalidOperationException();

            if (0 != (_xState & SvnExtendedState.MimeTypesLoaded))
                return;

            _xState = _xState | SvnExtendedState.MimeTypesLoaded;

            // The following code matches those in subversion/subversion/main.c
            // Note: We allocate everything in the context pool

            sbyte* mimetypes_file;

            using (var sp = new AprPool(_pool))
            {
                var cfgPtr = apr_hash.apr_hash_get(
                    CtxHandle.config,
                    new IntPtr(sp.AllocString(Constants.SVN_CONFIG_CATEGORY_CONFIG)),
                    Constants.APR_HASH_KEY_STRING);

                var cfg = svn_config_t.__CreateInstance(cfgPtr);

                svn_config.svn_config_get(
                    cfg,
                    &mimetypes_file,
                    sp.AllocString(Constants.SVN_CONFIG_SECTION_MISCELLANY),
                    sp.AllocString(Constants.SVN_CONFIG_OPTION_MIMETYPES_FILE),
                    null);
            }

            if (mimetypes_file != null && *mimetypes_file != 0)
            {
                apr_hash_t.__Internal* mimetypes_map = null;

                svn_error_t err = svn_io.svn_io_parse_mimetypes_file((void**) &mimetypes_map, mimetypes_file, _pool.Handle);

                CtxHandle.mimetypes_map = apr_hash_t.__CreateInstance(new IntPtr(mimetypes_map));

                if (err != null)
                {
                    var exception =
                        new SvnClientConfigurationException(SharpSvnStrings.LoadingMimeTypesMapFileFailed, SvnException.Create(err));

                    var ee = new SvnErrorEventArgs(exception);

                    HandleClientError(ee);

                    if (!ee.Cancel)
                        throw exception;
                }
            }
        }

        unsafe void ApplyUserDiffConfig(AprPool tmpPool)
        {
            if (_useUserDiff || CtxHandle.config == null)
                return;

            var cfgPtr = apr_hash.apr_hash_get(
                CtxHandle.config,
                new IntPtr(tmpPool.AllocString(Constants.SVN_CONFIG_CATEGORY_CONFIG)),
                Constants.APR_HASH_KEY_STRING);

            if (cfgPtr == IntPtr.Zero)
                return;

            var cfg = svn_config_t.__CreateInstance(cfgPtr);

            svn_config.svn_config_set(
                cfg,
                tmpPool.AllocString(Constants.SVN_CONFIG_SECTION_HELPERS),
                tmpPool.AllocString(Constants.SVN_CONFIG_OPTION_DIFF_CMD),
                null);

            svn_config.svn_config_set(
                cfg,
                tmpPool.AllocString(Constants.SVN_CONFIG_SECTION_HELPERS),
                tmpPool.AllocString(Constants.SVN_CONFIG_OPTION_DIFF3_CMD),
                null);
        }

        void LoadTortoiseSvnHooks()
        {
            throw new NotSupportedException("This version of LibSvnSharp doesn't support running TortoiseSVN hooks.");
        }

        /// <summary>Loads the subversion configuration from the specified path</summary>
        public void LoadConfiguration(string path)
        {
            LoadConfiguration(path, false);
        }

        /// <summary>Loads the subversion configuration from the specified path and optionally ensures the path is a subversion config dir by creating default files</summary>
        public unsafe void LoadConfiguration(string path, bool ensurePath)
        {
            if (_parent != null)
                throw new InvalidOperationException();

            if (State >= SvnContextState.ConfigPrepared)
                throw new InvalidOperationException("Configuration already loaded");

            if (string.IsNullOrEmpty(path))
                path = null;

            using (var tmpPool = new AprPool(_pool))
            {
                sbyte* szPath = path != null ? tmpPool.AllocAbsoluteDirent(path) : null;

                svn_error_t error;

                if (ensurePath)
                {
                    error = svn_config.svn_config_ensure(szPath, tmpPool.Handle);
                    if (error != null)
                        throw SvnException.Create(error);
                }

                apr_hash_t.__Internal* cfgPtr = null;

                error = svn_config.svn_config_get_config((void**) &cfgPtr, szPath, _pool.Handle);
                if (error != null)
                    throw SvnException.Create(error);

                var cfg = cfgPtr != null ? apr_hash_t.__CreateInstance(new IntPtr(cfgPtr)) : null;

                if (cfg != null)
                {
                    var configPtr = apr_hash.apr_hash_get(
                        cfg,
                        new IntPtr(tmpPool.AllocString(Constants.SVN_CONFIG_CATEGORY_CONFIG)),
                        Constants.APR_HASH_KEY_STRING);

                    var config = svn_config_t.__CreateInstance(configPtr);

                    sbyte* memoryCacheSizeStr;

                    svn_config.svn_config_get(
                        config,
                        &memoryCacheSizeStr,
                        tmpPool.AllocString(Constants.SVN_CONFIG_SECTION_MISCELLANY),
                        tmpPool.AllocString(Constants.SVN_CONFIG_OPTION_MEMORY_CACHE_SIZE),
                        null);

                    if (memoryCacheSizeStr != null)
                    {
                        svn_config.svn_config_set(
                            config,
                            tmpPool.AllocString(Constants.SVN_CONFIG_SECTION_MISCELLANY),
                            tmpPool.AllocString(Constants.SVN_CONFIG_OPTION_MEMORY_CACHE_SIZE),
                            null);
                    }
                }

                CtxHandle.config = cfg;

                _contextState = SvnContextState.ConfigPrepared;

                if (_configPath == null)
                    _configPath = path;
            }
        }

        /// <summary>Loads the standard subversion configuration and ensures the subversion config dir by creating default files</summary>
        public void LoadConfigurationDefault()
        {
            LoadConfiguration(null, true);
        }

        internal unsafe void EnsureState(SvnContextState requiredState)
        {
            if (_pool == null)
                throw new ObjectDisposedException("SvnClient");

            if (_parent != null)
            {
                _parent.EnsureState(requiredState);
                return;
            }

            if (State < SvnContextState.ConfigPrepared && requiredState >= SvnContextState.ConfigPrepared)
            {
                LoadConfigurationDefault();

                System.Diagnostics.Debug.Assert(State == SvnContextState.ConfigPrepared);
            }

            if (State < SvnContextState.ConfigLoaded && requiredState >= SvnContextState.ConfigLoaded)
            {
                using (var sp = new AprPool(_pool))
                {
                    ApplyUserDiffConfig(sp);

                    var configPtr = apr_hash.apr_hash_get(
                        CtxHandle.config,
                        new IntPtr(sp.AllocString(Constants.SVN_CONFIG_CATEGORY_SERVERS)),
                        Constants.APR_HASH_KEY_STRING);

                    if (configPtr != IntPtr.Zero)
                    {
                        var config = svn_config_t.__CreateInstance(configPtr);
                        int trustDefaultCa = 0;

                        svn_error_t err = svn_config.svn_config_get_bool(
                            config,
                            ref trustDefaultCa,
                            sp.AllocString(Constants.SVN_CONFIG_SECTION_GLOBAL),
                            sp.AllocString(Constants.SVN_CONFIG_OPTION_SSL_TRUST_DEFAULT_CA),
                            false);

                        if (err == null && trustDefaultCa == 0)
                        {
                            svn_config.svn_config_set_bool(
                                config,
                                sp.AllocString(Constants.SVN_CONFIG_SECTION_GLOBAL),
                                sp.AllocString(Constants.SVN_CONFIG_OPTION_SSL_TRUST_DEFAULT_CA),
                                false);
                        }
                        else
                            svn_error.svn_error_clear(err);
                    }

                    if (_pool != null && _configOverrides != null && _configOverrides.Count > 0)
                    {
                        foreach (SvnConfigItem item in _configOverrides)
                        {
                            var cfgPtr = apr_hash.apr_hash_get(
                                CtxHandle.config,
                                new IntPtr(sp.AllocString(item.File)),
                                Constants.APR_HASH_KEY_STRING);

                            if (cfgPtr == IntPtr.Zero)
                                continue;

                            var cfg = svn_config_t.__CreateInstance(cfgPtr);
                            svn_config.svn_config_set(
                                cfg,
                                _pool.AllocString(item.Section),
                                _pool.AllocString(item.Option),
                                _pool.AllocString(item.Value));
                        }
                    }
                }

                _contextState = SvnContextState.ConfigLoaded;
            }

            if (requiredState >= SvnContextState.AuthorizationInitialized)
            {
                if (State < SvnContextState.AuthorizationInitialized)
                {
                    int authCookie = 0;

                    CtxHandle.auth_baton = Authentication.GetAuthorizationBaton(ref authCookie);
                    _authCookie = authCookie;

                    _contextState = SvnContextState.AuthorizationInitialized;
                }
                else
                {
                    if (_authCookie != Authentication.Cookie)
                    {
                        // Authenticator configuration has changed; reload the baton and its backend

                        _contextState = SvnContextState.ConfigLoaded;
                        CtxHandle.auth_baton = null;

                        int authCookie = 0;

                        CtxHandle.auth_baton = Authentication.GetAuthorizationBaton(ref authCookie);
                        _authCookie = authCookie;

                        _contextState = SvnContextState.AuthorizationInitialized;
                    }
                }

                System.Diagnostics.Debug.Assert(State == SvnContextState.AuthorizationInitialized);
            }
        }

        internal void EnsureState(SvnContextState requiredState, SvnExtendedState xState)
        {
            if (_parent != null)
            {
                _parent.EnsureState(requiredState, xState);
                return;
            }

            EnsureState(requiredState);

            if (0 == (xState & ~_xState))
                return; // No changes to apply

            if (0 != ((xState & ~_xState) & SvnExtendedState.MimeTypesLoaded))
                ApplyMimeTypes();

            if (0 != ((xState & ~_xState) & SvnExtendedState.TortoiseSvnHooksLoaded))
                LoadTortoiseSvnHooks();
        }

        internal SvnContextState State => _contextState;

        /// <summary>Gets the <see cref="SvnAuthentication" /> instance managing authentication on behalf of this client</summary>
        public SvnAuthentication Authentication => _authentication;

        [ThreadStatic]
        internal static SvnClientContext _activeContext;
    }
}
