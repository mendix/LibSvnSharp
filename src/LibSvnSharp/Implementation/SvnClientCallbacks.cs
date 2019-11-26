using System;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Implementation
{
    sealed class SvnClientCallbacks : IDisposable
    {
        public readonly SafeFuncHandle<svn_cancel_func_t> libsvnsharp_cancel_func =
            new SafeFuncHandle<svn_cancel_func_t>(_libsvnsharp_cancel_func);

        static IntPtr _libsvnsharp_cancel_func(IntPtr cancelBaton)
        {
            var client = AprBaton<SvnClientContext>.Get(cancelBaton);

            SvnCancelEventArgs ea = new SvnCancelEventArgs();
            try
            {
                client.HandleClientCancel(ea);

                if (ea.Cancel)
                {
                    return svn_error.svn_error_create(
                        (int) SvnErrorCode.SVN_ERR_CANCELLED,
                        null,
                        "Operation canceled from OnCancel").__Instance;
                }

                return IntPtr.Zero;
            }
            catch (Exception e)
            {
                return SvnException.CreateExceptionSvnError("Cancel function", e).__Instance;
            }
            finally
            {
                ea.Detach(false);
            }
        }

        public readonly SafeFuncHandle<svn_ra_progress_notify_func_t> libsvnsharp_progress_func =
            new SafeFuncHandle<svn_ra_progress_notify_func_t>(_libsvnsharp_progress_func);

        static void _libsvnsharp_progress_func(long progress, long total, IntPtr baton, IntPtr pool)
        {
            var client = AprBaton<SvnClientContext>.Get(baton);

            SvnProgressEventArgs ea = new SvnProgressEventArgs(progress, total);

            try
            {
                client.HandleClientProgress(ea);
            }
            finally
            {
                ea.Detach(false);
            }
        }

        public readonly unsafe SafeFuncHandle<svn_client_get_commit_log3_t> libsvnsharp_commit_log_func =
            new SafeFuncHandle<svn_client_get_commit_log3_t>(_libsvnsharp_commit_log_func);

        static unsafe IntPtr _libsvnsharp_commit_log_func(
            sbyte** logMsg, sbyte** tmpFile, IntPtr commitItemsPtr, IntPtr baton, IntPtr pool)
        {
            var client = AprBaton<SvnClientContext>.Get(baton);

            var tmpPool = new AprPool(pool, false);

            var commit_items = apr_array_header_t.__CreateInstance(commitItemsPtr);

            var ea = new SvnCommittingEventArgs(commit_items, client.CurrentCommandArgs.CommandType, tmpPool);

            *logMsg = null;
            *tmpFile = null;

            try
            {
                client.HandleClientCommitting(ea);

                if (ea.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CANCELLED, null, "Operation canceled from OnCommitting").__Instance;
                else if (ea.LogMessage != null)
                    *logMsg = tmpPool.AllocUnixString(ea.LogMessage);
                else if (!client._noLogMessageRequired)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CANCELLED, null, "Commit canceled: A logmessage is required").__Instance;
                else
                    *logMsg = tmpPool.AllocString("");

                return IntPtr.Zero;
            }
            catch (Exception e)
            {
                return SvnException.CreateExceptionSvnError("Commit log", e).__Instance;
            }
            finally
            {
                ea.Detach(false);

                tmpPool.Dispose();
            }
        }

        public readonly SafeFuncHandle<svn_wc_notify_func2_t> svn_wc_notify_func2 =
            new SafeFuncHandle<svn_wc_notify_func2_t>(_svn_wc_notify_func2);

        static void _svn_wc_notify_func2(IntPtr baton, IntPtr notifyPtr, IntPtr pool)
        {
            var client = AprBaton<SvnClient>.Get(baton);
            var aprPool = new AprPool(pool, false);

            var notify = svn_wc_notify_t.__CreateInstance(notifyPtr);

            var ea = new SvnNotifyEventArgs(notify, client.CurrentCommandArgs.CommandType, aprPool);

            try
            {
                client.HandleClientNotify(ea);
            }
            finally
            {
                ea.Detach(false);

                aprPool.Dispose();
            }
        }

        public readonly unsafe SafeFuncHandle<svn_wc_conflict_resolver_func2_t> svn_wc_conflict_resolver_func =
            new SafeFuncHandle<svn_wc_conflict_resolver_func2_t>(_svn_wc_conflict_resolver_func);

        static unsafe IntPtr _svn_wc_conflict_resolver_func(
            void** resultPtr, IntPtr descriptionPtr, IntPtr baton, IntPtr resultPoolPtr, IntPtr scratchPoolPtr)
        {
            var client = AprBaton<SvnClient>.Get(baton);

            var conflictResult = svn_wc.svn_wc_create_conflict_result(
                svn_wc_conflict_choice_t.svn_wc_conflict_choose_postpone,
                null,
                apr_pool_t.__CreateInstance(resultPoolPtr));

            *resultPtr = conflictResult.__Instance.ToPointer();

            var resultPool = new AprPool(resultPoolPtr, false); // Connect to parent pool
            var scratchPool = new AprPool(scratchPoolPtr, false); // Connect to parent pool

            var description = svn_wc_conflict_description2_t.__CreateInstance(descriptionPtr);

            var ea = new SvnConflictEventArgs(description, scratchPool);

            try
            {
                client.HandleClientConflict(ea);

                if (ea.Cancel)
                    return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_CANCELLED, null, "Operation canceled from OnConflict").__Instance;

                conflictResult.choice = (svn_wc_conflict_choice_t)ea.Choice;

                if (ea.Choice == SvnAccept.Merged)
                {
                    if (ea.MergedValue != null)
                        conflictResult.merged_value = resultPool.AllocSvnString(ea.MergedValue);
                    if (ea.MergedFile != null)
                        conflictResult.merged_file = resultPool.AllocAbsoluteDirent(ea.MergedFile);
                }

                return IntPtr.Zero;
            }
            catch (Exception e)
            {
                return SvnException.CreateExceptionSvnError("Conflict resolver", e).__Instance;
            }
            finally
            {
                ea.Detach(false);

                scratchPool.Dispose();
                resultPool.Dispose();
            }
        }

        public void Dispose()
        {
            libsvnsharp_cancel_func.Dispose();
            libsvnsharp_progress_func.Dispose();
            libsvnsharp_commit_log_func.Dispose();
            svn_wc_notify_func2.Dispose();
            svn_wc_conflict_resolver_func.Dispose();
        }
    }
}
