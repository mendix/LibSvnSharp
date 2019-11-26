using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    interface ISvnAuthWrapper : IDisposable
    {
        svn_auth_provider_object_t GetProviderPtr(AprPool pool);

        int RetryLimit { get; set; }

        SvnAuthentication Authentication { get; }
    }
}
