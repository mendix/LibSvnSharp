using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Apr.Delegates;

namespace LibSvnSharp
{
    sealed class SvnExceptionContainer : IDisposable
    {
        const long _idValue = 0xFF00FFEE;

        AprBaton<SvnExceptionContainer> _baton;
        long _id;
        Exception _exception;

        public SvnExceptionContainer(Exception ex, apr_pool_t pool)
        {
            _id = _idValue;
            _exception = ex;

            _baton = new AprBaton<SvnExceptionContainer>(this);

            apr_pools.apr_pool_cleanup_register(
                pool,
                _baton.Handle,
                _cleanupHandle.Get(),
                _cleanupNullHandle.Get());

            _exception = ex;
        }

        public IntPtr Handle => _baton.Handle;

        public static SvnExceptionContainer Get(IntPtr handle)
        {
            return AprBaton<SvnExceptionContainer>.Get(handle);
        }

        public static Exception Fetch(SvnExceptionContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            System.Diagnostics.Debug.Assert(container != null && container._id == _idValue);

            return container._id == _idValue ? container._exception : null;
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.Assert(_id == _idValue);
            _id = 0;
            _exception = null;
            _baton.Dispose();
            _cleanupHandle.Dispose();
            _cleanupNullHandle.Dispose();
        }

        readonly SafeFuncHandle<Func_int_IntPtr> _cleanupHandle =
            new SafeFuncHandle<Func_int_IntPtr>(cleanup_handler);

        static int cleanup_handler(IntPtr data)
        {
            var ptr = AprBaton<SvnExceptionContainer>.Get(data);

            System.Diagnostics.Debug.Assert(ptr._id == _idValue);

            if (ptr._id == _idValue)
            {
                ptr.Dispose();
            }

            return 0;
        }

        readonly SafeFuncHandle<Func_int_IntPtr> _cleanupNullHandle =
            new SafeFuncHandle<Func_int_IntPtr>(apr_pools.__Internal.apr_pool_cleanup_null);
    }
}
