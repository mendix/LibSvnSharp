using System.Runtime.CompilerServices;
using LibSvnSharp.Interop.Apr;

// ReSharper disable once CheckNamespace
namespace LibSvnSharp.Interop.Svn
{
    // ReSharper disable once InconsistentNaming
    partial class svn_pools
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static apr_pool_t svn_pool_create(apr_pool_t parent) => svn_pool_create_ex(parent, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void svn_pool_clear(apr_pool_t pool) => apr_pools.apr_pool_clear(pool);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void svn_pool_destroy(apr_pool_t pool) => apr_pools.apr_pool_destroy(pool);
    }
}
