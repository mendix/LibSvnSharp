using System;
using System.Collections.ObjectModel;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        // List Client Command

        /// <summary>Streamingly lists directory entries in the repository. (<c>svn list</c>)</summary>
        public bool List(SvnTarget target, EventHandler<SvnListEventArgs> listHandler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (listHandler == null)
                throw new ArgumentNullException(nameof(listHandler));

            return List(target, new SvnListArgs(), listHandler);
        }

        /// <summary>Streamingly lists directory entries in the repository. (<c>svn list</c>)</summary>
        public unsafe bool List(SvnTarget target, SvnListArgs args, EventHandler<SvnListEventArgs> listHandler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            // We allow a null listHandler; the args object might just handle it itself

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            args.Prepare(target, args.Revision.RevisionType != SvnRevisionType.None);

            if (listHandler != null)
                args.List += listHandler;

            try
            {
                svn_opt_revision_t pegrev = target.Revision.AllocSvnRevision(pool);
                svn_opt_revision_t rev = args.Revision.Or(target.Revision).AllocSvnRevision(pool);

                using var svnclient_list_func_handle = new SafeFuncHandle<svn_client_list_func2_t>(svnclient_list_handler);

                svn_error_t r = svn_client.svn_client_list3(
                    target.AllocAsString(pool),
                    pegrev,
                    rev,
                    (svn_depth_t)args.Depth,
                    (uint)args.RetrieveEntries,
                    args.RetrieveLocks,
                    args.IncludeExternals,
                    svnclient_list_func_handle.Get(),
                    _clientBaton.Handle,
                    CtxHandle,
                    pool.Handle);

                return args.HandleResult(this, r, target);
            }
            finally
            {
                if (listHandler != null)
                    args.List -= listHandler;
            }
        }

        /// <summary>Gets a list of directory entries in the repository. (<c>svn list</c>)</summary>
        public bool GetList(SvnTarget target, out Collection<SvnListEventArgs> list)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var results = new InfoItemCollection<SvnListEventArgs>();

            try
            {
                return List(target, new SvnListArgs(), results.Handler);
            }
            finally
            {
                list = results;
            }
        }

        /// <summary>Gets a list of directory entries in the repository. (<c>svn list</c>)</summary>
        public bool GetList(SvnTarget target, SvnListArgs args, out Collection<SvnListEventArgs> list)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var results = new InfoItemCollection<SvnListEventArgs>();

            try
            {
                return List(target, args, results.Handler);
            }
            finally
            {
                list = results;
            }
        }

        static unsafe IntPtr svnclient_list_handler(
            IntPtr baton,
            sbyte* path,
            IntPtr dirent,
            IntPtr @lock,
            sbyte* absPath,
            sbyte* externalParentUrl,
            sbyte* externalTarget,
            IntPtr pool)
        {
            var client = AprBaton<SvnClient>.Get(baton);

            if (!(client.CurrentCommandArgs is SvnListArgs args))
                return IntPtr.Zero;

            var e = new SvnListEventArgs(
                path,
                svn_dirent_t.__CreateInstance(dirent),
                svn_lock_t.__CreateInstance(@lock),
                absPath,
                args.CalculateRepositoryRoot(absPath),
                externalParentUrl,
                externalTarget);

            try
            {
                args.OnList(e);

                if (e.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CEASE_INVOCATION, null, "List receiver canceled operation").__Instance;
                else
                    return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                return SvnException.CreateExceptionSvnError("List receiver", ex).__Instance;
            }
            finally
            {
                e.Detach(false);
            }
        }
    }
}
