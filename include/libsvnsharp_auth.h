#ifndef LIBSVNSHARP_AUTH_H
#define LIBSVNSHARP_AUTH_H


#include <apr_pools.h>
#include <apr_hash.h>

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */


/* The main auth baton. */
struct svn_auth_baton_t
{
  /* a collection of tables.  maps cred_kind -> provider_set */
  apr_hash_t *tables;

  /* the pool I'm allocated in. */
  apr_pool_t *pool;

  /* run-time parameters needed by providers. */
  apr_hash_t *parameters;
  apr_hash_t *slave_parameters;

  /* run-time credentials cache. */
  apr_hash_t *creds_cache;
};


#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* LIBSVNSHARP_AUTH_H */
