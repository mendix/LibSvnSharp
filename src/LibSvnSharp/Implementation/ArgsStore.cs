using System;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp.Implementation
{
    // Used as auto-dispose class for setting the _currentArgs property
    sealed class ArgsStore : IDisposable
    {
        readonly SvnClientContext _client;
        readonly SvnClientContext _lastContext;
        readonly svn_wc_context_t _wcCtx;

        public unsafe ArgsStore(SvnClientContext client, SvnClientArgs args, AprPool pool)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (client._currentArgs != null)
                throw new InvalidOperationException(SharpSvnStrings.SvnClientOperationInProgress);

            args.Prepare();
            client._currentArgs = args;
            _client = client;

            var ctx = _client.CtxHandle;
            _wcCtx = ctx.wc_ctx;

            {
                svn_client__private_ctx_t pctx = libsvnsharp_client.svn_client__get_private_ctx(ctx);
                pctx.total_progress = 0;
            }

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

                client.HandleProcessing(new SvnProcessingEventArgs(args.CommandType));
            }
            catch (Exception)
            {
                client._currentArgs = null;
                SvnClientContext._activeContext = _lastContext;
                throw;
            }
        }

        public void Dispose()
        {
            SvnClientContext._activeContext = _lastContext;
            SvnClientArgs args = _client._currentArgs;

            if (args != null)
                args._hooked = false;

            _client._currentArgs = null;

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
