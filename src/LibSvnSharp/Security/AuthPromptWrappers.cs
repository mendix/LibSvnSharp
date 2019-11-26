using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    sealed unsafe class AuthPromptWrappers : IDisposable
    {
        public readonly SafeFuncHandle<svn_auth_username_prompt_func_t> svn_auth_username_prompt_func =
            new SafeFuncHandle<svn_auth_username_prompt_func_t>(_svn_auth_username_prompt_func);

        static unsafe /*svn_error_t*/ IntPtr _svn_auth_username_prompt_func(
            /*out svn_auth_cred_username_t*/ void** credPtr, IntPtr baton, sbyte* realm, int maySave, /*apr_pool_t*/ IntPtr pool)
        {
            var wrapper = AprBaton<SvnAuthWrapper<SvnUserNameEventArgs>>.Get(baton);

            var args = new SvnUserNameEventArgs(SvnBase.Utf8_PtrToString(realm), maySave != 0);

            var cred = (svn_auth_cred_username_t.__Internal**) credPtr;

            using (var tmpPool = new AprPool(pool, false))
            {
                *cred = null;
                try
                {
                    wrapper.Raise(args);
                }
                catch (Exception e)
                {
                    return SvnException.CreateExceptionSvnError("Authorization handler", e).__Instance;
                }

                if (args.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CANCELLED, null, "Authorization canceled operation").__Instance;
                if (args.Break)
                    return IntPtr.Zero;

                *cred = (svn_auth_cred_username_t.__Internal*) tmpPool.AllocCleared(sizeof(svn_auth_cred_username_t.__Internal));

                (*cred)->username = new IntPtr(tmpPool.AllocString(args.UserName));
                (*cred)->may_save = args.Save ? 1 : 0;

                return IntPtr.Zero;
            }
        }

        public readonly SafeFuncHandle<svn_auth_simple_prompt_func_t> svn_auth_simple_prompt_func =
            new SafeFuncHandle<svn_auth_simple_prompt_func_t>(_svn_auth_simple_prompt_func);

        static unsafe /*svn_error_t*/ IntPtr _svn_auth_simple_prompt_func(
            /*out svn_auth_cred_simple_t*/ void** credPtr, IntPtr baton, sbyte* realm, sbyte* username, int maySave, /*apr_pool_t*/ IntPtr pool)
        {
            var wrapper = AprBaton<SvnAuthWrapper<SvnUserNamePasswordEventArgs>>.Get(baton);

            var args = new SvnUserNamePasswordEventArgs(SvnBase.Utf8_PtrToString(username), SvnBase.Utf8_PtrToString(realm), maySave != 0);

            var cred = (svn_auth_cred_simple_t.__Internal**) credPtr;

            using (var tmpPool = new AprPool(pool, false))
            {
                *cred = null;
                try
                {
                    wrapper.Raise(args);
                }
                catch (Exception e)
                {
                    return SvnException.CreateExceptionSvnError("Authorization handler", e).__Instance;
                }

                if (args.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CANCELLED, null, "Authorization canceled operation").__Instance;
                if (args.Break)
                    return IntPtr.Zero;

                *cred = (svn_auth_cred_simple_t.__Internal*) tmpPool.AllocCleared(sizeof(svn_auth_cred_simple_t.__Internal));

                (*cred)->username = new IntPtr(tmpPool.AllocString(args.UserName));
                (*cred)->password = new IntPtr(tmpPool.AllocString(args.Password));
                (*cred)->may_save = args.Save ? 1 : 0;
            }

            return IntPtr.Zero;
        }

        public readonly SafeFuncHandle<svn_auth_ssl_server_trust_prompt_func_t> svn_auth_ssl_server_trust_prompt_func =
            new SafeFuncHandle<svn_auth_ssl_server_trust_prompt_func_t>(_svn_auth_ssl_server_trust_prompt_func);

        static unsafe /*svn_error_t*/ IntPtr _svn_auth_ssl_server_trust_prompt_func(
            /*out svn_auth_cred_ssl_server_trust_t*/ void** ppCred, IntPtr baton, sbyte* realm, uint failures, /*svn_auth_ssl_server_cert_info_t*/ IntPtr certInfo, int maySave, /*apr_pool_t*/ IntPtr pool)
        {
            var wrapper = AprBaton<SvnAuthWrapper<SvnSslServerTrustEventArgs>>.Get(baton);
            var certInfoT = svn_auth_ssl_server_cert_info_t.__CreateInstance(certInfo);

            var args = new SvnSslServerTrustEventArgs(
                (SvnCertificateTrustFailures) failures,
                SvnBase.Utf8_PtrToString(certInfoT.hostname),
                SvnBase.Utf8_PtrToString(certInfoT.fingerprint),
                SvnBase.Utf8_PtrToString(certInfoT.valid_from),
                SvnBase.Utf8_PtrToString(certInfoT.valid_until),
                SvnBase.Utf8_PtrToString(certInfoT.issuer_dname),
                SvnBase.Utf8_PtrToString(certInfoT.ascii_cert),
                SvnBase.Utf8_PtrToString(realm), maySave != 0);

            var cred = (svn_auth_cred_ssl_server_trust_t.__Internal**) ppCred;

            using (var tmpPool = new AprPool(pool, false))
            {
                *cred = null;
                try
                {
                    wrapper.Raise(args);
                }
                catch (Exception e)
                {
                    return SvnException.CreateExceptionSvnError("Authorization handler", e).__Instance;
                }

                if (args.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CANCELLED, null, "Authorization canceled operation").__Instance;
                if (args.Break)
                    return IntPtr.Zero;

                if (args.AcceptedFailures != SvnCertificateTrustFailures.None)
                {
                    *cred = (svn_auth_cred_ssl_server_trust_t.__Internal*) tmpPool.AllocCleared(
                        sizeof(svn_auth_cred_ssl_server_trust_t.__Internal));

                    (*cred)->accepted_failures = (uint) args.AcceptedFailures;
                    (*cred)->may_save = args.Save ? 1 : 0;
                }
            }

            return IntPtr.Zero;
        }

        public readonly SafeFuncHandle<svn_auth_ssl_client_cert_prompt_func_t> svn_auth_ssl_client_cert_prompt_func =
            new SafeFuncHandle<svn_auth_ssl_client_cert_prompt_func_t>(_svn_auth_ssl_client_cert_prompt_func);

        static unsafe /*svn_error_t*/ IntPtr _svn_auth_ssl_client_cert_prompt_func(
            /*out svn_auth_cred_ssl_client_cert_t*/ void** credPtr, IntPtr baton, sbyte* realm, int maySave, /*apr_pool_t*/ IntPtr pool)
        {
            var wrapper = AprBaton<SvnAuthWrapper<SvnSslClientCertificateEventArgs>>.Get(baton);

            var args = new SvnSslClientCertificateEventArgs(SvnBase.Utf8_PtrToString(realm), maySave != 0);

            var cred = (svn_auth_cred_ssl_client_cert_t.__Internal**) credPtr;

            using (var tmpPool = new AprPool(pool, false))
            {
                *cred = null;
                try
                {
                    wrapper.Raise(args);
                }
                catch (Exception e)
                {
                    return SvnException.CreateExceptionSvnError("Authorization handler", e).__Instance;
                }

                if (args.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CANCELLED, null, "Authorization canceled operation").__Instance;
                if (args.Break)
                    return IntPtr.Zero;

                *cred = (svn_auth_cred_ssl_client_cert_t.__Internal*) tmpPool.AllocCleared(
                    sizeof(svn_auth_cred_ssl_client_cert_t.__Internal));

                (*cred)->cert_file = new IntPtr(tmpPool.AllocString(args.CertificateFile));
                (*cred)->may_save = args.Save ? 1 : 0;
            }

            return IntPtr.Zero;
        }

        public readonly SafeFuncHandle<svn_auth_plaintext_prompt_func_t> libsvnsharp_auth_plaintext_prompt =
            new SafeFuncHandle<svn_auth_plaintext_prompt_func_t>(_libsvnsharp_auth_plaintext_prompt);

        static unsafe IntPtr _libsvnsharp_auth_plaintext_prompt(int* maySavePlaintext, sbyte* realmString, IntPtr baton, IntPtr pool)
        {
            *maySavePlaintext = 1;
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            svn_auth_username_prompt_func.Dispose();
            svn_auth_simple_prompt_func.Dispose();
            svn_auth_ssl_server_trust_prompt_func.Dispose();
            svn_auth_ssl_client_cert_prompt_func.Dispose();
            libsvnsharp_auth_plaintext_prompt.Dispose();
        }
    }
}
