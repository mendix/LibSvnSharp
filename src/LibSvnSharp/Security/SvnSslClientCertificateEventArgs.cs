using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    public class SvnSslClientCertificateEventArgs : SvnAuthenticationEventArgs
    {
        string _certificateFile;

        public SvnSslClientCertificateEventArgs(string realm, bool maySave)
            : base(realm, maySave)
        {
        }

        public string CertificateFile
        {
            get => _certificateFile ?? "";
            set => _certificateFile = value;
        }

        protected internal override void Clear()
        {
            base.Clear();
            _certificateFile = null;
        }

        internal sealed class Wrapper : SvnAuthWrapper<SvnSslClientCertificateEventArgs>
        {
            public Wrapper(EventHandler<SvnSslClientCertificateEventArgs> handler, SvnAuthentication authentication)
                : base(handler, authentication)
            {
            }

            public override unsafe svn_auth_provider_object_t GetProviderPtr(AprPool pool)
            {
                if (pool == null)
                    throw new ArgumentNullException(nameof(pool));

                svn_auth_provider_object_t.__Internal* provider = null;

                if (_handler.Equals(SvnAuthentication.SubversionFileSslClientCertificateHandler))
                {
                    svn_auth.svn_auth_get_ssl_client_cert_file_provider((void**) &provider, pool.Handle);
                }
                else
                {
                    svn_auth.svn_auth_get_ssl_client_cert_prompt_provider(
                        (void**) &provider,
                        _callbacks.svn_auth_ssl_client_cert_prompt_func.Get(),
                        _baton.Handle,
                        RetryLimit,
                        pool.Handle);
                }

                return svn_auth_provider_object_t.__CreateInstance(new IntPtr(provider));
            }
        }
    }
}
