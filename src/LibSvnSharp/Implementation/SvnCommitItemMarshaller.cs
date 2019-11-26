using System;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Implementation
{
    sealed class SvnCommitItemMarshaller : IItemMarshaller<SvnCommitItem>
    {
        public unsafe int ItemSize => sizeof(svn_client_commit_item3_t.__Internal*);

        public void Write(SvnCommitItem value, IntPtr ptr, AprPool pool)
        {
            throw new NotImplementedException();
        }

        public unsafe SvnCommitItem Read(IntPtr ptr, AprPool pool)
        {
            var ppcCommitItem = (svn_client_commit_item3_t.__Internal**)ptr.ToPointer();

            var pcCommitItem = svn_client_commit_item3_t.__CreateInstance(new IntPtr(*ppcCommitItem));

            return new SvnCommitItem(pcCommitItem, pool);
        }
    }
}
