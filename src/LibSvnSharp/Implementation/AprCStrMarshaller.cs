using System;

namespace LibSvnSharp.Implementation
{
    sealed unsafe class AprCStrMarshaller : IItemMarshaller<string>
    {
        public int ItemSize => sizeof(sbyte*);

        public void Write(string value, IntPtr ptr, AprPool pool)
        {
            var ppStr = (sbyte**) ptr;

            *ppStr = pool.AllocString(value);
        }

        public string Read(IntPtr ptr, AprPool pool)
        {
            var ppcStr = (sbyte**) ptr;

            return SvnBase.Utf8_PtrToString(*ppcStr);
        }
    }
}
