using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <summary>Gets log messages of the specified target</summary>
        public bool Log(Uri target, EventHandler<SvnLogEventArgs> logHandler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return Log(target, new SvnLogArgs(), logHandler);
        }

        /// <summary>Gets log messages of the specified target</summary>
        public bool Log(Uri target, SvnLogArgs args, EventHandler<SvnLogEventArgs> logHandler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InternalLog(NewSingleItemCollection(UriToStringCanonical(target)), null, SvnRevision.Head, args, logHandler);
        }

        /// <summary>Gets log messages of the specified target</summary>
        public bool GetLog(Uri target, out Collection<SvnLogEventArgs> logItems)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var results = new InfoItemCollection<SvnLogEventArgs>();

            try
            {
                return Log(target, results.Handler);
            }
            finally
            {
                logItems = results;
            }
        }

        /// <summary>Gets log messages of the specified target</summary>
        public bool GetLog(Uri target, SvnLogArgs args, out Collection<SvnLogEventArgs> logItems)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var results = new InfoItemCollection<SvnLogEventArgs>();

            try
            {
                return Log(target, args, results.Handler);
            }
            finally
            {
                logItems = results;
            }
        }

        unsafe bool InternalLog(ICollection<string> targets, Uri searchRoot, SvnRevision altPegRev, SvnLogArgs args, EventHandler<SvnLogEventArgs> logHandler)
        {
            if (targets == null)
                throw new ArgumentNullException(nameof(targets));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            args._mergeLogLevel = 0; // Clear log level
            args._searchRoot = searchRoot;

            if (logHandler != null)
                args.Log += logHandler;

            try
            {
                apr_array_header_t retrieveProperties;

                if (args.RetrieveAllProperties)
                    retrieveProperties = null;
                else if (args.RetrievePropertiesUsed)
                    retrieveProperties = AllocArray(args.RetrieveProperties, pool);
                else
                    retrieveProperties = svn_compat.svn_compat_log_revprops_in(pool.Handle);

                svn_opt_revision_t pegRev = args.OriginRevision.Or(altPegRev).AllocSvnRevision(pool);

                int count = args.RangesUsed ? args.Ranges.Count : 1;
                var revisionRanges = apr_tables.apr_array_make(
                    pool.Handle, count, sizeof(svn_opt_revision_range_t.__Internal*));

                if (args.RangesUsed)
                {
                    foreach (SvnRevisionRange r in args.Ranges)
                    {
                        var range = (svn_opt_revision_range_t.__Internal*)pool.Alloc(
                            sizeof(svn_opt_revision_range_t.__Internal));

                        range->start = r.StartRevision.Or(SvnRevision.Head).ToSvnRevision();
                        range->end = r.EndRevision.Or(SvnRevision.Zero).ToSvnRevision();

                        *((svn_opt_revision_range_t.__Internal**)apr_tables.apr_array_push(revisionRanges)) = range;
                    }
                }
                else
                {
                    var range = (svn_opt_revision_range_t.__Internal*) pool.Alloc(
                        sizeof(svn_opt_revision_range_t.__Internal));

                    range->start = args.Start.Or(args.OriginRevision).Or(SvnRevision.Head).ToSvnRevision();
                    range->end = args.End.Or(SvnRevision.Zero).ToSvnRevision();

                    *((svn_opt_revision_range_t.__Internal**) apr_tables.apr_array_push(revisionRanges)) = range;
                }

                using var svnclient_log_receiver_handle = new SafeFuncHandle<svn_log_entry_receiver_t>(svnclient_log_handler);

                svn_error_t err = svn_client.svn_client_log5(
                    AllocArray(targets, pool),
                    pegRev,
                    revisionRanges,
                    args.Limit,
                    args.RetrieveChangedPaths,
                    args.StrictNodeHistory,
                    args.RetrieveMergedRevisions,
                    retrieveProperties,
                    svnclient_log_receiver_handle.Get(),
                    _clientBaton.Handle,
                    CtxHandle,
                    pool.Handle);

                return args.HandleResult(this, err, targets);
            }
            finally
            {
                if (logHandler != null)
                    args.Log -= logHandler;

                args._searchRoot = null;
                args._mergeLogLevel = 0;
            }
        }

        [Obsolete("Apparently this is almost the same as SvnBase.UriToCanonicalString")]
        static string UriToStringCanonical(Uri value)
        {
            if (value == null)
                return null;

            var name = UriToString(value);

            if (!string.IsNullOrEmpty(name) && name[name.Length - 1] == '/')
                return name.TrimEnd('/'); // "svn://host:port" is canoncialized to "svn://host:port/" by the .Net Uri class

            return name;
        }

        static IntPtr svnclient_log_handler(IntPtr baton, IntPtr logEntryPtr, IntPtr pool)
        {
            var client = AprBaton<SvnClient>.Get(baton);

            if (!(client.CurrentCommandArgs is SvnLogArgs args))
                return IntPtr.Zero;

            var logEntry = svn_log_entry_t.__CreateInstance(logEntryPtr);

            if (logEntry.revision == -1)
            {
                // This marks the end of logs at this level,
                args._mergeLogLevel--;
                return IntPtr.Zero;
            }

            using AprPool aprPool = new AprPool(pool, false);

            var e = new SvnLogEventArgs(logEntry, args._mergeLogLevel, args._searchRoot, aprPool);

            if (logEntry.has_children)
                args._mergeLogLevel++;

            try
            {
                args.OnLog(e);

                if (e.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CEASE_INVOCATION, null, "Log receiver canceled operation").__Instance;
                else
                    return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                return SvnException.CreateExceptionSvnError("Log receiver", ex).__Instance;
            }
            finally
            {
                e.Detach(false);
            }
        }
    }
}
