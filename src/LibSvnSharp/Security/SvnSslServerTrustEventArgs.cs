using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    public class SvnSslServerTrustEventArgs : SvnAuthenticationEventArgs
    {
        SvnCertificateTrustFailures _acceptedFailures;

        public SvnSslServerTrustEventArgs(
            SvnCertificateTrustFailures failures,
            string certificateCommonName,
            string certificateFingerprint,
            string certificateValidFrom,
            string certificateValidUntil,
            string certificateIssuer,
            string certificateValue,
            string realm,
            bool maySave)
            : base(realm, maySave)
        {
            Failures = failures;
            CommonName = certificateCommonName ?? throw new ArgumentNullException(nameof(certificateCommonName));
            Fingerprint = certificateFingerprint ?? throw new ArgumentNullException(nameof(certificateFingerprint));
            ValidFrom = certificateValidFrom ?? throw new ArgumentNullException(nameof(certificateValidFrom));
            ValidUntil = certificateValidUntil ?? throw new ArgumentNullException(nameof(certificateValidUntil));
            Issuer = certificateIssuer ?? throw new ArgumentNullException(nameof(certificateIssuer));
            CertificateValue = certificateValue ?? throw new ArgumentNullException(nameof(certificateValue));
        }

        public SvnCertificateTrustFailures Failures { get; }

        public SvnCertificateTrustFailures AcceptedFailures
        {
            get => _acceptedFailures;
            set => _acceptedFailures = value & SvnCertificateTrustFailures.MaskAllFailures;
        }

        /// <summary>Common name of the certificate</summary>
        public string CommonName { get; }

        /// <summary>Fingerprint name of the certificate</summary>
        public string Fingerprint { get; }

        /// <summary>Text valid-from value of the certificate</summary>
        public string ValidFrom { get; }

        /// <summary>Text valid-until value of the certificate</summary>
        public string ValidUntil { get; }

        /// <summary>Issuer value of the certificate</summary>
        public string Issuer { get; }

        /// <summary>Text version of the certificate</summary>
        public string CertificateValue { get; }

        protected internal override void Clear()
        {
            base.Clear();
            _acceptedFailures = SvnCertificateTrustFailures.None;
        }

        internal sealed class Wrapper : SvnAuthWrapper<SvnSslServerTrustEventArgs>
        {
            public Wrapper(EventHandler<SvnSslServerTrustEventArgs> handler, SvnAuthentication authentication)
                : base(handler, authentication)
            {
            }

            public override unsafe svn_auth_provider_object_t GetProviderPtr(AprPool pool)
            {
                if (pool == null)
                    throw new ArgumentNullException(nameof(pool));

                svn_auth_provider_object_t.__Internal* provider = null;

                if (_handler.Equals(SvnAuthentication.SubversionFileSslServerTrustHandler))
                {
                    svn_auth.svn_auth_get_ssl_server_trust_file_provider((void**) &provider, pool.Handle);
                }
                else if (_handler.Equals(SvnAuthentication.SubversionWindowsSslServerTrustHandler))
                {
                    var err = svn_auth.svn_auth_get_platform_specific_provider(
                        (void**) &provider,
                        provider_name: "windows",
                        provider_type: "ssl_server_trust",
                        pool.Handle);

                    if (err != null)
                        throw SvnException.Create(err);
                }
                else
                {
                    svn_auth.svn_auth_get_ssl_server_trust_prompt_provider(
                        (void**) &provider,
                        _callbacks.svn_auth_ssl_server_trust_prompt_func.Get(),
                        _baton.Handle,
                        pool.Handle);
                }

                return svn_auth_provider_object_t.__CreateInstance(new IntPtr(provider));
            }
        }
    }
}
