using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Retrieves the value of a property on files, dirs, or revisions (<c>svn propget</c>)</overloads>
        /// <summary>Gets the specified property from the specfied path</summary>
        /// <returns>true if property is set, otherwise false</returns>
        /// <exception type="SvnException">path is not a valid workingcopy path</exception>
        public bool GetProperty(SvnTarget target, string propertyName, out string value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            value = null;

            bool ok = GetProperty(target, propertyName, new SvnGetPropertyArgs(), out var result);

            if (ok && result != null && (result.Count > 0))
                value = result[0].StringValue;

            return ok;
        }

        /// <summary>Gets the specified property from the specfied path</summary>
        /// <returns>true if property is set, otherwise false</returns>
        /// <exception type="SvnException">path is not a valid workingcopy path</exception>
        public bool GetProperty(SvnTarget target, string propertyName, out SvnPropertyValue value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            value = null;

            if (GetProperty(target, propertyName, new SvnGetPropertyArgs(), out var result))
            {
                if (result.Count != 0)
                    value = result[0];

                return true;
            }

            return false;
        }

        /// <summary>Sets the specified property on the specfied path to value</summary>
        /// <remarks>Use <see cref="DeleteProperty(string, string, SvnSetPropertyArgs)" /> to remove an existing property</remarks>
        public unsafe bool GetProperty(SvnTarget target, string propertyName, SvnGetPropertyArgs args, out SvnTargetPropertyCollection properties)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            properties = null;
            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            var pegRev = target.Revision.AllocSvnRevision(pool);
            var rev = args.Revision.Or(target.Revision).AllocSvnRevision(pool);
            int actualRev = 0;

            apr_hash_t.__Internal* pHash = null;

            sbyte* pName = pool.AllocString(propertyName);

            sbyte* prefix = null;
            sbyte* targetName = target.AllocAsString(pool);

            if (!svn_path.svn_path_is_url(targetName))
            {
                prefix = targetName;

                targetName = target.AllocAsString(pool, true);
            }

            svn_error_t r = svn_client.svn_client_propget5(
                (void**) &pHash,
                null,
                pName,
                targetName,
                pegRev,
                rev,
                ref actualRev,
                (svn_depth_t) args.Depth,
                CreateChangeListsList(args.ChangeLists, pool), // Intersect ChangeLists
                CtxHandle,
                pool.Handle,
                pool.Handle);

            if (pHash != null)
            {
                var rd = new SvnTargetPropertyCollection();

                apr_hash_t hash = apr_hash_t.__CreateInstance(new IntPtr(pHash));

                for (apr_hash_index_t hi = apr_hash.apr_hash_first(pool.Handle, hash); hi != null; hi = apr_hash.apr_hash_next(hi))
                {
                    sbyte* pKey;
                    long keyLen = 0;
                    svn_string_t.__Internal* propVal;

                    apr_hash.apr_hash_this(hi, (void**) &pKey, ref keyLen, (void**) &propVal);

                    SvnTarget itemTarget;
                    if (prefix != null && !svn_path.svn_path_is_url(pKey))
                    {
                        string path = Utf8_PathPtrToString(
                            svn_dirent_uri.svn_dirent_join(
                                prefix,
                                svn_dirent_uri.svn_dirent_skip_ancestor(targetName, pKey),
                                pool.Handle),
                            pool);

                        if (!string.IsNullOrEmpty(path))
                            itemTarget = path;
                        else
                            itemTarget = ".";
                    }
                    else
                        itemTarget = Utf8_PtrToUri(pKey, SvnNodeKind.Unknown);

                    var propValStr = svn_string_t.__CreateInstance(new IntPtr(propVal));
                    rd.Add(SvnPropertyValue.Create(pName, propValStr, itemTarget, propertyName));
                }

                properties = rd;
            }

            return args.HandleResult(this, r, target);
        }

        /// <summary>Tries to get a property from the specified path (<c>svn propget</c>)</summary>
        /// <remarks>Eats all (non-argument) exceptions</remarks>
        /// <returns>True if the property is fetched, otherwise false</returns>
        /// <remarks>Equivalent to GetProperty with <see cref="SvnGetPropertyArgs" />'s ThrowOnError set to false</remarks>
        public bool TryGetProperty(SvnTarget target, string propertyName, out string value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            value = null;

            var args = new SvnGetPropertyArgs();
            args.ThrowOnError = false;

            if (GetProperty(target, propertyName, args, out var result))
            {
                if (result.Count > 0)
                {
                    value = result[0].StringValue;

                    return (value != null);
                }

                // Fall through if no property fetched
            }
            return false;
        }
    }
}
