using System;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Implementation
{
    [Obsolete]
    unsafe sealed class AprCStrDirentMarshaller : IItemMarshaller<string>
    {
        public int ItemSize => sizeof(sbyte*);

        public void Write(string value, IntPtr ptr, AprPool pool)
        {
            sbyte** ppStr = (sbyte**)ptr;

            *ppStr = pool.AllocDirent(value);
        }

        public string Read(IntPtr ptr, AprPool pool)
        {
            sbyte** ppcStr = (sbyte**)ptr;

            return SvnBase.Utf8_PtrToString(svn_dirent_uri.svn_dirent_local_style(*ppcStr, pool.Handle));
        }
    }
}
