using System;
using System.Runtime.InteropServices;

namespace LibSvnSharp.Implementation
{
    class AprBaton<T> : IDisposable
    {
        GCHandle _handle;

        public AprBaton(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _handle = GCHandle.Alloc(value, GCHandleType.WeakTrackResurrection);
        }

        public IntPtr Handle => GCHandle.ToIntPtr(_handle);

        public void Dispose()
        {
            Destroy();
            GC.SuppressFinalize(this);
        }

        public static T Get(IntPtr value)
        {
            return (T) GCHandle.FromIntPtr(value).Target;
        }

        public static unsafe T Get(void* ptr)
        {
            return (T) GCHandle.FromIntPtr(new IntPtr(ptr)).Target;
        }

        void Destroy()
        {
            if (_handle.IsAllocated)
                _handle.Free();
        }

        ~AprBaton()
        {
            Destroy();
        }
    }
}
