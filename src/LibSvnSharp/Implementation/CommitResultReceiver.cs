using System;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Implementation
{
    sealed class CommitResultReceiver : IDisposable
    {
        AprBaton<CommitResultReceiver> _commitBaton;
        SvnClientContext _client;

        public CommitResultReceiver(SvnClientContext client)
        {
            CommitCallback = new SafeFuncHandle<svn_commit_callback2_t>(the_commit_callback2);
            _commitBaton = new AprBaton<CommitResultReceiver>(this);
            _client = client;
        }

        static IntPtr the_commit_callback2(IntPtr commit_info_ptr, IntPtr baton, IntPtr pool)
        {
            var tmpPool = new AprPool(pool, false);
            var receiver = AprBaton<CommitResultReceiver>.Get(baton);

            try
            {
                var commit_info = svn_commit_info_t.__CreateInstance(commit_info_ptr);

                receiver.ProvideCommitResult(commit_info, tmpPool);

                return IntPtr.Zero;
            }
            catch (Exception e)
            {
                return SvnException.CreateExceptionSvnError("CommitResult function", e).__Instance;
            }
            finally
            {
                tmpPool.Dispose();
            }
        }

        public void Dispose()
        {
            _client = null;
            CommitCallback.Dispose();
            CommitCallback = null;
            _commitBaton.Dispose();
            _commitBaton = null;
        }

        internal SafeFuncHandle<svn_commit_callback2_t> CommitCallback { get; private set; }

        internal IntPtr CommitBaton => _commitBaton.Handle;

        internal void ProvideCommitResult(svn_commit_info_t commit_info, AprPool pool)
        {
            CommitResult = SvnCommittedEventArgs.Create(_client, commit_info, pool);

            _client.HandleClientCommitted(CommitResult);
        }

        public SvnCommittedEventArgs CommitResult { get; private set; }
    }
}
