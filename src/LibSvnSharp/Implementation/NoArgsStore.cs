using System;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp.Implementation
{
    // Used as auto-dispose class for setting a temporary wc_ctx
    sealed class NoArgsStore : IDisposable
    {
        readonly SvnClientContext _client;
        readonly SvnClientContext _lastContext;
        readonly svn_wc_context_t _wcCtx;

        public unsafe NoArgsStore(SvnClientContext client, AprPool pool)
        {
            if (client._currentArgs != null)
                throw new InvalidOperationException(SharpSvnStrings.SvnClientOperationInProgress);

            _client = client;

            var ctx = _client.CtxHandle;
            _wcCtx = ctx.wc_ctx;

            _lastContext = SvnClientContext._activeContext;
            SvnClientContext._activeContext = _client;

            try
            {
                if (!client.KeepSession && pool != null)
                {
                    svn_wc_context_t.__Internal* p_wc_ctx = null;

                    var error = svn_wc.svn_wc_context_create((void**) &p_wc_ctx, null, pool.Handle, pool.Handle);
                    if (error != null)
                        throw SvnException.Create(error);

                    ctx.wc_ctx = svn_wc_context_t.__CreateInstance(new IntPtr(p_wc_ctx));
                }
            }
            catch (Exception)
            {
                SvnClientContext._activeContext = _lastContext;
                throw;
            }
        }

        public void Dispose()
        {
            SvnClientContext._activeContext = _lastContext;

            svn_client_ctx_t ctx = _client.CtxHandle;
            ctx.wc_ctx = _wcCtx;

            /*
            SvnSshContext ssh = _client._sshContext;
            if (ssh != null)
                ssh.OperationCompleted(_client.KeepSession);
            */
        }
    }
}
