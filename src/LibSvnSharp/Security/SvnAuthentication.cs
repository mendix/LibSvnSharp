using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    /// <summary>Container for all subversion authentication settings on a client</summary>
    public class SvnAuthentication : SvnBase
    {
        readonly SvnClientContext _clientContext;
        readonly Dictionary<Delegate, ISvnAuthWrapper> _wrappers;
        readonly List<ISvnAuthWrapper> _handlers;
        readonly AprPool _authPool;
        readonly AprPool _parentPool;
        ICredentials _defaultCredentials;
        //SvnCredentialWrapper _credentialWrapper;
        bool _readOnly;
        int _cookie;
        svn_auth_baton_t _currentBaton;
        string _forcedUser;
        string _forcedPassword;

        internal SvnAuthentication(SvnClientContext context, AprPool parentPool)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _wrappers = new Dictionary<Delegate, ISvnAuthWrapper>();
            _handlers = new List<ISvnAuthWrapper>();

            _parentPool = parentPool;
            _authPool = new AprPool(_parentPool);
            _clientContext = context;

            AddSubversionFileHandlers(); // Add handlers which use no interaction by default
        }

        internal int Cookie => _cookie;

        public event EventHandler<SvnUserNamePasswordEventArgs> UserNamePasswordHandlers
        {
            add
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                ISvnAuthWrapper handler = new SvnUserNamePasswordEventArgs.Wrapper(value, this);

                if (!_wrappers.ContainsKey(value))
                {
                    _cookie++;
                    _wrappers.Add(value, handler);
                    _handlers.Add(handler);
                }
            }
            remove
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                if (_wrappers.TryGetValue(value, out var wrapper))
                {
                    _cookie++;
                    _wrappers.Remove(value);
                    _handlers.Remove(wrapper);
                }
            }
        }

        public event EventHandler<SvnUserNameEventArgs> UserNameHandlers
        {
            add
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                ISvnAuthWrapper handler = new SvnUserNameEventArgs.Wrapper(value, this);

                if (!_wrappers.ContainsKey(value))
                {
                    _cookie++;
                    _wrappers.Add(value, handler);
                    _handlers.Add(handler);
                }
            }
            remove
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                if (_wrappers.TryGetValue(value, out var wrapper))
                {
                    _cookie++;
                    _wrappers.Remove(value);
                    _handlers.Remove(wrapper);
                }
            }
        }

        public event EventHandler<SvnSslServerTrustEventArgs> SslServerTrustHandlers
        {
            add
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                ISvnAuthWrapper handler = new SvnSslServerTrustEventArgs.Wrapper(value, this);

                if (!_wrappers.ContainsKey(value))
                {
                    _cookie++;
                    _wrappers.Add(value, handler);
                    _handlers.Add(handler);
                }
            }
            remove
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                if (_wrappers.TryGetValue(value, out var wrapper))
                {
                    _cookie++;
                    _wrappers.Remove(value);
                    _handlers.Remove(wrapper);
                }
            }
        }

        /*
        public event EventHandler<SvnSslAuthorityTrustEventArgs> SslAuthorityTrustHandlers
        {
            add
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                else if (value == null)
                    return;

                ISvnAuthWrapper handler = new SvnSslAuthorityTrustEventArgs.Wrapper(value, this);

                if (!_wrappers.ContainsKey(value))
                {
                    _cookie++;
                    _wrappers.Add(value, handler);
                    _handlers.Add(handler);
                }
            }
            remove
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                else if (value == null)
                    return;

                ISvnAuthWrapper wrapper;

                if (_wrappers.TryGetValue(value, wrapper))
                {
                    _cookie++;
                    _wrappers.Remove(value);
                    _handlers.Remove(wrapper);
                }
            }
        }
        */

        public event EventHandler<SvnSslClientCertificateEventArgs> SslClientCertificateHandlers
        {
            add
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                ISvnAuthWrapper handler = new SvnSslClientCertificateEventArgs.Wrapper(value, this);

                if (!_wrappers.ContainsKey(value))
                {
                    _cookie++;
                    _wrappers.Add(value, handler);
                    _handlers.Add(handler);
                }
            }

            remove
            {
                if (_readOnly)
                    throw new InvalidOperationException();
                if (value == null)
                    return;

                if (_wrappers.TryGetValue(value, out var wrapper))
                {
                    _cookie++;
                    _wrappers.Remove(value);
                    _handlers.Remove(wrapper);
                }
            }
        }

        /*
        public event EventHandler<SvnSslClientCertificatePasswordEventArgs^>^ SslClientCertificatePasswordHandlers
        {
            void add(EventHandler<SvnSslClientCertificatePasswordEventArgs^>^ e)
            {
                if (_readOnly)
                    throw gcnew InvalidOperationException();
                else if (!e)
                    return;

                ISvnAuthWrapper^ handler = gcnew SvnSslClientCertificatePasswordEventArgs::Wrapper(e, this);

                if (!_wrappers->ContainsKey(e))
                {
                    _cookie++;
                    _wrappers->Add(e, handler);
                    _handlers->Add(handler);
                }
            }

            void remove(EventHandler<SvnSslClientCertificatePasswordEventArgs^>^ e)
            {
                if (_readOnly)
                    throw gcnew InvalidOperationException();
                else if (!e)
                    return;

                ISvnAuthWrapper^ wrapper;

                if (_wrappers->TryGetValue(e, wrapper))
                {
                    _cookie++;
                    _wrappers->Remove(e);
                    _handlers->Remove(wrapper);
                }
            }
        }

        public event EventHandler<SvnSshServerTrustEventArgs^>^ SshServerTrustHandlers
        {
            void add(EventHandler<SvnSshServerTrustEventArgs^>^ e)
            {
                if (_readOnly)
                    throw gcnew InvalidOperationException();
                else if (!e)
                    return;

                ISvnAuthWrapper^ handler = gcnew SvnSshServerTrustEventArgs::Wrapper(e, this);

                if (!_wrappers->ContainsKey(e))
                {
                    //_cookie++;
                    _wrappers->Add(e, handler);
                    _handlers->Add(handler);
                }
            }

            void remove(EventHandler<SvnSshServerTrustEventArgs^>^ e)
            {
                if (_readOnly)
                    throw gcnew InvalidOperationException();
                else if (!e)
                    return;

                ISvnAuthWrapper^ wrapper;

                if (_wrappers->TryGetValue(e, wrapper))
                {
                    //_cookie++;
                    _wrappers->Remove(e);
                    _handlers->Remove(wrapper);
                }
            }
        }
        */

        /// <summary>Subversion UserNameHandler file backend (managed representation)</summary>
        public static EventHandler<SvnUserNameEventArgs> SubversionFileUserNameHandler { get; } = ImpSubversionFileUserNameHandler;

        /// <summary>Subversion UserNamePasswordHandler file backend (managed representation)</summary>
        public static EventHandler<SvnUserNamePasswordEventArgs> SubversionFileUserNamePasswordHandler { get; } = ImpSubversionFileUserNamePasswordHandler;

        /// <summary>Subversion UserNameHandler file backend using Windows CryptoStore (managed representation)</summary>
        /// <remarks>Should be added after <see cref="SubversionFileUserNamePasswordHandler" /></remarks>
        public static EventHandler<SvnUserNamePasswordEventArgs> SubversionWindowsUserNamePasswordHandler { get; } = ImpSubversionWindowsFileUserNamePasswordHandler;

        /// <summary>Subversion SslServerTrust file backend (managed representation)</summary>
        public static EventHandler<SvnSslServerTrustEventArgs> SubversionFileSslServerTrustHandler { get; } = ImpSubversionFileSslServerTrustHandler;

        /// <summary>Subversion SslClientCertificate file backend (managed representation)</summary>
        public static EventHandler<SvnSslClientCertificateEventArgs> SubversionFileSslClientCertificateHandler { get; } = ImpSubversionFileSslClientCertificateHandler;

        /// <summary>Subversion CryptoApi Ssl Trust handler</summary>
        public static EventHandler<SvnSslServerTrustEventArgs> SubversionWindowsSslServerTrustHandler { get; } = ImpSubversionWindowsSslServerTrustHandler;

        /// <summary>Adds all default subversion-configuration-based handlers</summary>
        public void AddSubversionFileHandlers()
        {
            UserNameHandlers += SubversionFileUserNameHandler;
            UserNamePasswordHandlers += SubversionFileUserNamePasswordHandler;
            SslServerTrustHandlers += SubversionFileSslServerTrustHandler;
            SslClientCertificateHandlers += SubversionFileSslClientCertificateHandler;
            //SslClientCertificatePasswordHandlers += SubversionFileSslClientCertificatePasswordHandler;
            AddWindowsSubversionAuthenticationHandlers();
        }

        void AddWindowsSubversionAuthenticationHandlers()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            UserNamePasswordHandlers += SubversionWindowsUserNamePasswordHandler;
            SslServerTrustHandlers += SubversionWindowsSslServerTrustHandler;
            //SslAuthorityTrustHandlers += SubversionWindowsSslAuthorityTrustHandler;
            //SslClientCertificatePasswordHandlers += SubversionWindowsSslClientCertificatePasswordHandler;
        }

        /// <summary>Retrieves an authorization baton allocated in the specified pool; containing the current authorization settings</summary>
        internal unsafe svn_auth_baton_t GetAuthorizationBaton(ref int cookie)
        {
            if (_currentBaton != null && _cookie == cookie)
                return _currentBaton;

            using var tmpPool = new AprPool(_parentPool);

            apr_hash_t creds = null;

            if (_currentBaton != null)
                creds = clone_credentials(get_cache(_currentBaton), null, tmpPool);

            _authPool.Clear();

            var authArray = new AprArray<ISvnAuthWrapper, SvnAuthProviderMarshaller>(_handlers, _authPool);

            svn_auth_baton_t.__Internal* rsltPtr = null;

            svn_auth.svn_auth_open((void**) &rsltPtr, authArray.Handle, _authPool.Handle);

            var rslt = svn_auth_baton_t.__CreateInstance(new IntPtr(rsltPtr));

            if (creds != null)
                clone_credentials(creds, get_cache(rslt), _authPool);

            if (_clientContext._configPath != null)
            {
                svn_auth.svn_auth_set_parameter(
                    rslt,
                    tmpPool.AllocString(Constants.SVN_AUTH_PARAM_CONFIG_DIR),
                    new IntPtr(_authPool.AllocDirent(_clientContext._configPath)));
            }

            if (_forcedUser != null)
            {
                svn_auth.svn_auth_set_parameter(
                    rslt,
                    tmpPool.AllocString(Constants.SVN_AUTH_PARAM_DEFAULT_USERNAME),
                    new IntPtr(_authPool.AllocString(_forcedUser)));
            }

            if (_forcedPassword != null)
            {
                svn_auth.svn_auth_set_parameter(
                    rslt,
                    tmpPool.AllocString(Constants.SVN_AUTH_PARAM_DEFAULT_PASSWORD),
                    new IntPtr(_authPool.AllocString(_forcedPassword)));
            }

            _currentBaton = rslt;

            cookie = _cookie;
            return rslt;
        }

        static void ImpSubversionFileUserNameHandler(object sender, SvnUserNameEventArgs e) => e.Break = true;

        static void ImpSubversionFileUserNamePasswordHandler(object sender, SvnUserNamePasswordEventArgs e) => e.Break = true;

        static void ImpSubversionWindowsFileUserNamePasswordHandler(object sender, SvnUserNamePasswordEventArgs e) => e.Break = true;

        static void ImpSubversionFileSslServerTrustHandler(object sender, SvnSslServerTrustEventArgs e) => e.Break = true;

        static void ImpSubversionWindowsSslServerTrustHandler(object sender, SvnSslServerTrustEventArgs e) => e.Break = true;

        static void ImpSubversionFileSslClientCertificateHandler(object sender, SvnSslClientCertificateEventArgs e) => e.Break = true;

        static apr_hash_t get_cache(svn_auth_baton_t baton)
        {
            if (baton == null)
                throw new ArgumentNullException(nameof(baton));

            return baton.creds_cache;
        }

        static unsafe apr_hash_t clone_credentials(apr_hash_t from, apr_hash_t to, AprPool pool)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));
            // to can be null

            apr_hash_t hash_to = to ?? apr_hash.apr_hash_make(pool.Handle);
            sbyte* pKey;
            long len = default;
            void* pValue;
            void* pNewValue;

            using (var tmpPool = new AprPool(pool))
            {
                for (var hi = apr_hash.apr_hash_first(tmpPool.Handle, from); hi != null; hi = apr_hash.apr_hash_next(hi))
                {
                    apr_hash.apr_hash_this(hi, (void**) &pKey, ref len, &pValue);

                    pNewValue = null;

                    if (pValue == null)
                        continue;

#if IMPLEMENTED
                    if (MatchPrefix(pKey, Constants.SVN_AUTH_CRED_SIMPLE))
                        pNewValue = clone_cred_usernamepassword((const svn_auth_cred_simple_t*)pValue, pool);
                    else if (MatchPrefix(pKey, Constants.SVN_AUTH_CRED_USERNAME))
                        pNewValue = clone_cred_username((const svn_auth_cred_username_t*)pValue, pool);
                    else if (MatchPrefix(pKey, Constants.SVN_AUTH_CRED_SSL_CLIENT_CERT))
                        pNewValue = clone_cred_ssl_clientcert((const svn_auth_cred_ssl_client_cert_t*)pValue, pool);
                    else if (MatchPrefix(pKey, Constants.SVN_AUTH_CRED_SSL_CLIENT_CERT_PW))
                        pNewValue = clone_cred_ssl_clientcertpw((const svn_auth_cred_ssl_client_cert_pw_t*)pValue, pool);
                    else if (MatchPrefix(pKey, Constants.SVN_AUTH_CRED_SSL_SERVER_TRUST))
                        pNewValue = clone_cred_ssl_servercert((const svn_auth_cred_ssl_server_trust_t*)pValue, pool);
                    /* else: Unknown -> Don't copy */

                    if (pNewValue != null)
                    {
                        // Create a 0 terminated copy of key
                        char *pNewKey = (char*)apr_pcalloc(pool->Handle, len+1);
                        memcpy(pNewKey, pKey, len);
                        apr_hash_set(hash_to, pNewKey, len, pNewValue);
                    }
#endif // IMPLEMENTED

                    throw new NotImplementedException();
                }
            }

            return hash_to;
        }

#if IMPLEMENTED
        static bool _MatchPrefix(string key, string needle)
        {
            return key.StartsWith(needle, StringComparison.Ordinal);
        }

        static unsafe bool MatchPrefix(sbyte* key, string needle)
        {
            return _MatchPrefix(new string(key), needle + ":");
        }
#endif // IMPLEMENTED
    }
}
