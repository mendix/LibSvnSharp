using System;

namespace LibSvnSharp.Implementation
{
    unsafe sealed class AprUriMarshaller : IItemMarshaller<string>, IItemMarshaller<Uri>
    {
        public int ItemSize => sizeof(sbyte*);

        void IItemMarshaller<string>.Write(string value, IntPtr ptr, AprPool pool)
        {
            sbyte** ppStr = (sbyte**)ptr.ToPointer();
            *ppStr = pool.AllocUri(value);
        }

        string IItemMarshaller<string>.Read(IntPtr ptr, AprPool pool)
        {
            sbyte** ppcStr = (sbyte**)ptr.ToPointer();

            return SvnBase.Utf8_PtrToString(*ppcStr);
        }

        void IItemMarshaller<Uri>.Write(Uri value, IntPtr ptr, AprPool pool)
        {
            sbyte** ppStr = (sbyte**)ptr.ToPointer();
            *ppStr = pool.AllocUri(value);
        }

        Uri IItemMarshaller<Uri>.Read(IntPtr ptr, AprPool pool)
        {
            sbyte** ppcStr = (sbyte**)ptr.ToPointer();

            return SvnBase.Utf8_PtrToUri(*ppcStr, SvnNodeKind.Unknown);
        }
    }
}
