using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;
using LibSvnSharp.Interop.Apr;

namespace LibSvnSharp.Implementation
{
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    abstract class SvnHandleBase : CriticalFinalizerObject
    {
        internal static volatile apr_pool_t _ultimateParentPool = null;

        static SvnHandleBase()
        {
            SvnBase.EnsureLoaded();
        }
    }
}
