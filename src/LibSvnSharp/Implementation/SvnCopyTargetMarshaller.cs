using System;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Implementation
{
    unsafe class SvnCopyTargetMarshaller : IItemMarshaller<SvnTarget>
    {
        public int ItemSize => sizeof(svn_client_copy_source_t.__Internal*);

        public void Write(SvnTarget value, IntPtr ptr, AprPool pool)
        {
            var srcObj = svn_client_copy_source_t.__CreateInstance(
                pool.AllocCleared(sizeof(svn_client_copy_source_t.__Internal)));

            var src = (svn_client_copy_source_t.__Internal**) ptr;
            *src = (svn_client_copy_source_t.__Internal*) srcObj.__Instance;

            srcObj.path = value.AllocAsString(pool, true);
            srcObj.revision = value.GetSvnRevision(SvnRevision.Working, SvnRevision.Head).AllocSvnRevision(pool);
            srcObj.peg_revision = value.GetSvnRevision(SvnRevision.Working, SvnRevision.Head).AllocSvnRevision(pool);
        }

        public SvnTarget Read(IntPtr ptr, AprPool pool)
        {
            //sbyte** ppcStr = (sbyte**) ptr.ToPointer();

            return null;
        }
    }
}
