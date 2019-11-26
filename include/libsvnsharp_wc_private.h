#ifndef LIBSVNSHARP_WC_PRIVATE_H
#define LIBSVNSHARP_WC_PRIVATE_H

#include <svn_wc.h>

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */


/*
 * Convert from svn_wc_status3_t to svn_wc_status2_t.
 * Allocate the result in RESULT_POOL.
 *
 * Deprecated because svn_wc_status2_t is deprecated and the only
 * calls are from other deprecated functions.
 */
SVN_DEPRECATED
svn_error_t *
svn_wc__status2_from_3(svn_wc_status2_t **status,
                       const svn_wc_status3_t *old_status,
                       svn_wc_context_t *wc_ctx,
                       const char *local_abspath,
                       apr_pool_t *result_pool,
                       apr_pool_t *scratch_pool);

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* LIBSVNSHARP_WC_PRIVATE_H */
