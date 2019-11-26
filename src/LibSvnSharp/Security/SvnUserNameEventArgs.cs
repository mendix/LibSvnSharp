using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    public class SvnUserNameEventArgs : SvnAuthenticationEventArgs, ISvnAuthenticationEventArgs
    {
        string _username;

        public SvnUserNameEventArgs(string initialUserName, string realm, bool maySave)
            : base(realm, maySave)
        {
            InitialUserName = initialUserName;
            _username = initialUserName ?? "";
        }

        public SvnUserNameEventArgs(string realm, bool maySave)
            : base(realm, maySave)
        {
            _username = "";
        }

        /// <summary>Default username; can be NULL</summary>
        public string InitialUserName { get; }

        /// <summary>The username to authorize with</summary>
        public string UserName
        {
            get => _username;
            set => _username = value ?? "";
        }

        string ISvnAuthenticationEventArgs.CredentialKind => Constants.SVN_AUTH_CRED_USERNAME;

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

            var cred = svn_auth_cred_username_t.__CreateInstance(credentials);

            UserName = cred.username != null ? SvnBase.Utf8_PtrToString(cred.username) : null;
            Save = cred.may_save;

            return true;
        }

        protected internal override void Clear()
        {
            base.Clear();
            _username = InitialUserName ?? "";
        }

        internal sealed class Wrapper : SvnAuthWrapper<SvnUserNameEventArgs>
        {
            public Wrapper(EventHandler<SvnUserNameEventArgs> handler, SvnAuthentication authentication)
                : base(handler, authentication)
            {
            }

            public override unsafe svn_auth_provider_object_t GetProviderPtr(AprPool pool)
            {
                if (pool == null)
                    throw new ArgumentNullException(nameof(pool));

                svn_auth_provider_object_t.__Internal* provider = null;

                if (_handler.Equals(SvnAuthentication.SubversionFileUserNameHandler))
                {
                    svn_auth.svn_auth_get_username_provider((void**) &provider, pool.Handle);
                }
                else
                {
                    svn_auth.svn_auth_get_username_prompt_provider(
                        (void**) &provider,
                        _callbacks.svn_auth_username_prompt_func.Get(),
                        _baton.Handle,
                        RetryLimit,
                        pool.Handle);
                }

                return svn_auth_provider_object_t.__CreateInstance(new IntPtr(provider));
            }
        }
    }
}
