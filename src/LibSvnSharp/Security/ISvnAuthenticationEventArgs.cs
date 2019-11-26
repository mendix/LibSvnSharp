using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Security
{
    interface ISvnAuthenticationEventArgs
    {
        string CredentialKind { get; }

        void Setup(svn_auth_baton_t auth_baton, AprPool pool);
        void Done(svn_auth_baton_t auth_baton, AprPool pool);
        bool Apply(IntPtr credentials);
    }
}
