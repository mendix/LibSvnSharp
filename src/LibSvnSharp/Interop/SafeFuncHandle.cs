using System;
using System.Runtime.InteropServices;

namespace LibSvnSharp.Interop
{
    sealed class SafeFuncHandle<T> : IDisposable
        where T : Delegate
    {
        readonly T _func;
        GCHandle _handle;

        public SafeFuncHandle(T func)
        {
            _func = func;
        }

        public T Get()
        {
            if (!_handle.IsAllocated)
                _handle = GCHandle.Alloc(_func);

            return _func;
        }

        public void Dispose()
        {
            Destroy();
            GC.SuppressFinalize(this);
        }

        void Destroy()
        {
            if (_handle.IsAllocated)
                _handle.Free();
        }

        ~SafeFuncHandle()
        {
            Destroy();
        }
    }
}
