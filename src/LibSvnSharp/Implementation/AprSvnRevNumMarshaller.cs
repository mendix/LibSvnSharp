using System;

namespace LibSvnSharp.Implementation
{
    sealed class AprSvnRevNumMarshaller : IItemMarshaller<long>
    {
        public int ItemSize => sizeof(long);

        public unsafe void Write(long value, IntPtr ptr, AprPool pool)
        {
            long* pRev = (long*)ptr;

            *pRev = value;
        }

        public unsafe long Read(IntPtr ptr, AprPool pool)
        {
            return *(long*)ptr;
        }
    }
}
