#ifndef LIBSVNSHARP_CLIENT_H
#define LIBSVNSHARP_CLIENT_H


#include <svn_client.h>

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */


/* Private client context.
 *
 * This is what is actually allocated by svn_client_create_context2(),
 * which then returns the address of the public_ctx member. */
typedef struct svn_client__private_ctx_t
{
  /* Reserved field, always zero, to detect misuse of the private
     context as a public client context. */
  apr_uint64_t magic_null;

  /* Reserved field, always set to a known magic number, to identify
     this struct as the private client context. */
  apr_uint64_t magic_id;

  /* Total number of bytes transferred over network across all RA sessions. */
  apr_off_t total_progress;

  /* The public context. */
  svn_client_ctx_t public_ctx;
} svn_client__private_ctx_t;


/* Given a public client context CTX, return the private context
   within which it is allocated. */
svn_client__private_ctx_t *
svn_client__get_private_ctx(svn_client_ctx_t *ctx);


#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* LIBSVNSHARP_CLIENT_H */
