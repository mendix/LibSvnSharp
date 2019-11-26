using System;
using System.Collections.ObjectModel;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <summary>Streamingly retrieves information about a local or remote item (<c>svn info</c>)</summary>
        public bool Info(SvnTarget target, EventHandler<SvnInfoEventArgs> infoHandler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (infoHandler == null)
                throw new ArgumentNullException(nameof(infoHandler));

            return Info(target, new SvnInfoArgs(), infoHandler);
        }

        /// <summary>Streamingly retrieves information about a local or remote item (<c>svn info</c>)</summary>
        public unsafe bool Info(SvnTarget target, SvnInfoArgs args, EventHandler<SvnInfoEventArgs> infoHandler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            // We allow a null infoHandler; the args object might just handle it itself

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            if (infoHandler != null)
                args.Info += infoHandler;

            try
            {
                var pegRev = target.GetSvnRevision(SvnRevision.None, SvnRevision.Head).AllocSvnRevision(pool);
                var rev = args.Revision.Or(target.GetSvnRevision(SvnRevision.None, SvnRevision.Head)).AllocSvnRevision(pool);

                using var svn_info_receiver_handle = new SafeFuncHandle<svn_client_info_receiver2_t>(svn_info_receiver);

                svn_error_t r = svn_client.svn_client_info4(
                    target.AllocAsString(pool, true),
                    pegRev,
                    rev,
                    (svn_depth_t) args.Depth,
                    args.RetrieveExcluded,
                    args.RetrieveActualOnly,
                    args.IncludeExternals,
                    CreateChangeListsList(args.ChangeLists, pool), // Intersect ChangeLists
                    svn_info_receiver_handle.Get(),
                    _clientBaton.Handle,
                    CtxHandle,
                    pool.Handle);

                return args.HandleResult(this, r, target);
            }
            finally
            {
                if (infoHandler != null)
                    args.Info -= infoHandler;
            }
        }

        /// <summary>Gets information about the specified target</summary>
        public bool GetInfo(SvnTarget target, out SvnInfoEventArgs info)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var results = new InfoItemCollection<SvnInfoEventArgs>();

            try
            {
                return Info(target, new SvnInfoArgs(), results.Handler);
            }
            finally
            {
                info = results.Count > 0 ? results[0] : null;
            }
        }

        /// <summary>Gets information about the specified target</summary>
        public bool GetInfo(SvnTarget target, SvnInfoArgs args, out Collection<SvnInfoEventArgs> info)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var results = new InfoItemCollection<SvnInfoEventArgs>();

            try
            {
                return Info(target, args, results.Handler);
            }
            finally
            {
                info = results;
            }
        }

        static unsafe IntPtr svn_info_receiver(IntPtr baton, sbyte* path, IntPtr info_ptr, IntPtr pool)
        {
            var client = AprBaton<SvnClient>.Get(baton);

            using var thePool = new AprPool(pool, false);

            if (!(client.CurrentCommandArgs is SvnInfoArgs args))
                return IntPtr.Zero;

            var info = svn_client_info2_t.__CreateInstance(info_ptr);
            var e = new SvnInfoEventArgs(SvnBase.Utf8_PathPtrToString(path, thePool), info, thePool);

            try
            {
                args.OnInfo(e);

                if (e.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CEASE_INVOCATION, null, "Info receiver canceled operation").__Instance;
                else
                    return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                return SvnException.CreateExceptionSvnError("Info receiver", ex).__Instance;
            }
            finally
            {
                e.Detach(false);
            }
        }
    }
}
