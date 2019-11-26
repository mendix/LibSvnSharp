using System;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    [Flags]
    public enum SvnCertificateTrustFailures
    {
        None                        = 0,
        CertificateNotValidYet      = svn_auth_ssl_enum_t.SVN_AUTH_SSL_NOTYETVALID,
        CertificateExpired          = svn_auth_ssl_enum_t.SVN_AUTH_SSL_EXPIRED,
        CommonNameMismatch          = svn_auth_ssl_enum_t.SVN_AUTH_SSL_CNMISMATCH,
        UnknownCertificateAuthority = svn_auth_ssl_enum_t.SVN_AUTH_SSL_UNKNOWNCA,

        UnknownSslProviderFailure   = svn_auth_ssl_enum_t.SVN_AUTH_SSL_OTHER,

        MaskAllFailures             = CertificateNotValidYet | CertificateExpired | CommonNameMismatch | UnknownCertificateAuthority | UnknownSslProviderFailure
    }
}
