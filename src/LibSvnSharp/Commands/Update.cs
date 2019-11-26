using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Bring changes from the repository into the working copy (<c>svn update</c>)</overloads>
        /// <summary>Recursively updates the specified path to the latest (HEAD) revision</summary>
        /// <exception type="SvnException">Operation failed</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Update(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            return Update(NewSingleItemCollection(path), new SvnUpdateArgs(), out _);
        }

        /// <summary>Recursively updates the specified path to the latest (HEAD) revision</summary>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Update(string path, out SvnUpdateResult updateResult)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            return Update(NewSingleItemCollection(path), new SvnUpdateArgs(), out updateResult);
        }

        /// <summary>Recursively updates the specified path</summary>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Update(string path, SvnUpdateArgs args)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Update(NewSingleItemCollection(path), args, out _);
        }

        /// <summary>Recursively updates the specified path to the latest (HEAD) revision</summary>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Update(string path, SvnUpdateArgs args, out SvnUpdateResult updateResult)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Update(NewSingleItemCollection(path), args, out updateResult);
        }

        /// <summary>Recursively updates the specified paths to the latest (HEAD) revision</summary>
        /// <exception type="SvnException">Operation failed</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Update(ICollection<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            return Update(paths, new SvnUpdateArgs(), out _);
        }

        /// <summary>Recursively updates the specified paths to the latest (HEAD) revision</summary>
        /// <exception type="SvnException">Operation failed</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Update(ICollection<string> paths, out SvnUpdateResult updateResult)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            return Update(paths, new SvnUpdateArgs(), out updateResult);
        }

        /// <summary>Updates the specified paths to the specified revision</summary>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        /// <returns>true if the operation succeeded; false if it did not</returns>
        public bool Update(ICollection<string> paths, SvnUpdateArgs args)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Update(paths, args, out _);
        }

        /// <summary>Updates the specified paths to the specified revision</summary>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        /// <returns>true if the operation succeeded; false if it did not</returns>
        public unsafe bool Update(ICollection<string> paths, SvnUpdateArgs args, out SvnUpdateResult updateResult)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            updateResult = null;

            foreach (string s in paths)
            {
                if (string.IsNullOrEmpty(s))
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(paths));
                if (!IsNotUri(s))
                    throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(paths));
            }

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            var aprPaths = new AprArray<string, AprCStrDirentMarshaller>(paths, pool);

            apr_array_header_t.__Internal* revs_ptr = null;
            svn_opt_revision_t uRev = args.Revision.Or(SvnRevision.Head).AllocSvnRevision(pool);

            svn_error_t r = svn_client.svn_client_update4(
                (void**) &revs_ptr,
                aprPaths.Handle,
                uRev,
                (svn_depth_t) args.Depth,
                args.KeepDepth,
                args.IgnoreExternals,
                args.AllowObstructions,
                args.AddsAsModifications,
                args.UpdateParents,
                CtxHandle,
                pool.Handle);

            if (args.HandleResult(this, r, paths))
            {
                var revs = apr_array_header_t.__CreateInstance(new IntPtr(revs_ptr));

                var aprRevs = new AprArray<long, AprSvnRevNumMarshaller>(revs, pool);

                updateResult = new SvnUpdateResult(paths, aprRevs.ToArray(), (paths.Count >= 1) ? aprRevs[0] : -1);

                return true;
            }

            return false;
        }
    }
}
