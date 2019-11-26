using System;

namespace LibSvnSharp.Implementation
{
    interface IItemMarshaller<T>
    {
        int ItemSize { get; }

        void Write(T value, IntPtr ptr, AprPool pool);

        T Read(IntPtr ptr, AprPool pool);
    }
}
