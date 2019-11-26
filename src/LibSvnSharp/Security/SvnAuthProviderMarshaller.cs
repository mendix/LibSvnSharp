using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    sealed class SvnAuthProviderMarshaller : IItemMarshaller<ISvnAuthWrapper>
    {
        public unsafe int ItemSize => sizeof(svn_auth_provider_object_t.__Internal*);

        public unsafe void Write(ISvnAuthWrapper value, IntPtr ptr, AprPool pool)
        {
            var ppProvider = (svn_auth_provider_object_t.__Internal**) ptr;

            *ppProvider = (svn_auth_provider_object_t.__Internal*) value.GetProviderPtr(pool).__Instance;

            if (*ppProvider == null)
            {
                var stub_provider = (svn_auth_provider_t.__Internal*) pool.AllocCleared(
                    sizeof(svn_auth_provider_t.__Internal));

                stub_provider->cred_kind = new IntPtr(pool.AllocString("LibSvnSharp-FakeType"));

                var pProvider = (svn_auth_provider_object_t.__Internal*) pool.AllocCleared(
                    sizeof(svn_auth_provider_object_t.__Internal));

                pProvider->vtable = new IntPtr(stub_provider);

                *ppProvider = pProvider;
            }
        }

        public ISvnAuthWrapper Read(IntPtr ptr, AprPool pool)
        {
            // Not needed; we only provide arrays; item is black box to us
            throw new NotImplementedException();
        }
    }
}
