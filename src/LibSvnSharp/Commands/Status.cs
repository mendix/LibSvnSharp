using System;
using System.Collections.ObjectModel;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Retrieve the status of working copy files and directories (<c>svn status</c>)</overloads>
        /// <summary>Recursively gets 'interesting' status data for the specified path</summary>
        public bool Status(string path, EventHandler<SvnStatusEventArgs> statusHandler)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));
            if (statusHandler == null)
                throw new ArgumentNullException(nameof(statusHandler));

            return Status(path, new SvnStatusArgs(), statusHandler);
        }

        /// <summary>Gets status data for the specified path</summary>
        public unsafe bool Status(string path, SvnStatusArgs args, EventHandler<SvnStatusEventArgs> statusHandler)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            // We allow a null statusHandler; the args object might just handle it itself

            if (args.ContactRepository)
                EnsureState(SvnContextState.AuthorizationInitialized);
            else
                EnsureState(SvnContextState.ConfigLoaded);

            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            if (statusHandler != null)
                args.Status += statusHandler;

            try
            {
                int version = 0;

                svn_opt_revision_t pegRev = args.Revision.AllocSvnRevision(pool);

                using var svnclient_status_func_handle = new SafeFuncHandle<svn_client_status_func_t>(svnclient_status_handler);

                svn_error_t r = svn_client.svn_client_status6(
                    ref version,
                    CtxHandle,
                    pool.AllocDirent(path),
                    pegRev,
                    (svn_depth_t) args.Depth,
                    args.RetrieveAllEntries,
                    args.RetrieveRemoteStatus,
                    !args.IgnoreWorkingCopyStatus,
                    args.RetrieveIgnoredEntries,
                    args.IgnoreExternals,
                    args.KeepDepth,
                    CreateChangeListsList(args.ChangeLists, pool), // Intersect ChangeLists
                    svnclient_status_func_handle.Get(),
                    _clientBaton.Handle,
                    pool.Handle);

                return args.HandleResult(this, r, path);
            }
            finally
            {
                if (statusHandler != null)
                    args.Status -= statusHandler;
            }
        }

        /// <summary>Recursively gets a list of 'interesting' status data for the specified path</summary>
        public bool GetStatus(string path, out Collection<SvnStatusEventArgs> statuses)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var results = new InfoItemCollection<SvnStatusEventArgs>();

            try
            {
                return Status(path, new SvnStatusArgs(), results.Handler);
            }
            finally
            {
                statuses = results;
            }
        }

        /// <summary>Gets a list of status data for the specified path</summary>
        public bool GetStatus(string path, SvnStatusArgs args, out Collection<SvnStatusEventArgs> statuses)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var results = new InfoItemCollection<SvnStatusEventArgs>();

            try
            {
                return Status(path, args, results.Handler);
            }
            finally
            {
                statuses = results;
            }
        }

        static unsafe IntPtr svnclient_status_handler(IntPtr baton, sbyte* path, IntPtr status_ptr, IntPtr scratch_pool)
        {
            var client = AprBaton<SvnClient>.Get(baton);

            using var aprPool = new AprPool(scratch_pool, false);

            if (!(client.CurrentCommandArgs is SvnStatusArgs args))
                return IntPtr.Zero;

            var status = svn_client_status_t.__CreateInstance(status_ptr);
            var e = new SvnStatusEventArgs(path, status, client, aprPool);

            try
            {
                args.OnStatus(e);

                if (e.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CEASE_INVOCATION, null, "Status receiver canceled operation").__Instance;
                else
                    return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                return SvnException.CreateExceptionSvnError("Status receiver", ex).__Instance;
            }
            finally
            {
                e.Detach(false);
            }
        }
    }
}
