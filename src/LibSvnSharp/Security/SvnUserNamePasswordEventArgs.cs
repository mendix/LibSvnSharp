using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    public class SvnUserNamePasswordEventArgs : SvnAuthenticationEventArgs, ISvnAuthenticationEventArgs
    {
        string _username;
        string _password;

        public SvnUserNamePasswordEventArgs(string initialUserName, string realm, bool maySave)
            : base(realm, maySave)
        {
            InitialUserName = initialUserName;
            _username = initialUserName ?? "";
            _password = "";
        }

        /// <summary>Default username; can be NULL</summary>
        public string InitialUserName { get; }

        /// <summary>The username to authorize with</summary>
        public string UserName
        {
            get => _username;
            set => _username = value ?? "";
        }

        /// <summary>The password to authorize with</summary>
        public string Password
        {
            get => _password;
            set => _password = value ?? "";
        }

        string ISvnAuthenticationEventArgs.CredentialKind => Constants.SVN_AUTH_CRED_SIMPLE;

        void ISvnAuthenticationEventArgs.Setup(svn_auth_baton_t auth_baton, AprPool pool)
        {
        }

        void ISvnAuthenticationEventArgs.Done(svn_auth_baton_t auth_baton, AprPool pool)
        {
        }

        unsafe bool ISvnAuthenticationEventArgs.Apply(IntPtr credentials)
        {
            if (credentials == IntPtr.Zero)
                return false;

            var cred = svn_auth_cred_simple_t.__CreateInstance(credentials);

            UserName = cred.username != null ? SvnBase.Utf8_PtrToString(cred.username) : null;
            Password = cred.password != null ? SvnBase.Utf8_PtrToString(cred.password) : null;
            Save = cred.may_save;

            return true;
        }

        protected internal override void Clear()
        {
            base.Clear();
            _username = InitialUserName ?? "";
            _password = "";
        }

        internal sealed class Wrapper : SvnAuthWrapper<SvnUserNamePasswordEventArgs>
        {
            public Wrapper(EventHandler<SvnUserNamePasswordEventArgs> handler, SvnAuthentication authentication)
                : base(handler, authentication)
            {
            }

            public override unsafe svn_auth_provider_object_t GetProviderPtr(AprPool pool)
            {
                if (pool == null)
                    throw new ArgumentNullException(nameof(pool));

                svn_auth_provider_object_t.__Internal* provider = null;

                if (_handler.Equals(SvnAuthentication.SubversionFileUserNamePasswordHandler))
                {
                    svn_auth.svn_auth_get_simple_provider2(
                        (void**) &provider,
                        _callbacks.libsvnsharp_auth_plaintext_prompt.Get(),
                        _baton.Handle,
                        pool.Handle);
                }
                else if (_handler.Equals(SvnAuthentication.SubversionWindowsUserNamePasswordHandler))
                {
                    var err = svn_auth.svn_auth_get_platform_specific_provider(
                        (void**) &provider,
                        provider_name: "windows",
                        provider_type: "simple",
                        pool.Handle);

                    if (err != null)
                        throw SvnException.Create(err);
                }
                else
                {
                    svn_auth.svn_auth_get_simple_prompt_provider(
                        (void**) &provider,
                        _callbacks.svn_auth_simple_prompt_func.Get(),
                        _baton.Handle,
                        RetryLimit,
                        pool.Handle);
                }

                return svn_auth_provider_object_t.__CreateInstance(new IntPtr(provider));
            }
        }
    }
}
