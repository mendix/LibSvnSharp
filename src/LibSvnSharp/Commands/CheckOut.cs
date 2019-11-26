using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Check out a working copy from a repository. (<c>svn checkout</c>)</overloads>
        /// <summary>Performs a recursive checkout of <paramref name="url" /> to <paramref name="path" /></summary>
        /// <exception type="SvnException">Operation failed</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool CheckOut(SvnUriTarget url, string path)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return CheckOut(url, path, new SvnCheckOutArgs(), out _);
        }

        /// <summary>Performs a recursive checkout of <paramref name="url" /> to <paramref name="path" /></summary>
        /// <exception type="SvnException">Operation failed</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool CheckOut(SvnUriTarget url, string path, out SvnUpdateResult result)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return CheckOut(url, path, new SvnCheckOutArgs(), out result);
        }

        /// <summary>Performs a checkout of <paramref name="url" /> to <paramref name="path" /> to the specified param</summary>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        /// <returns>true if the operation succeeded; false if it did not</returns>
        public bool CheckOut(SvnUriTarget url, string path, SvnCheckOutArgs args)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return CheckOut(url, path, args, out _);
        }

        /// <summary>Performs a checkout of <paramref name="url" /> to <paramref name="path" /> to the specified param</summary>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        /// <returns>true if the operation succeeded; false if it did not</returns>
        public unsafe bool CheckOut(SvnUriTarget url, string path, SvnCheckOutArgs args, out SvnUpdateResult result)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (args.Revision.RequiresWorkingCopy)
                throw new ArgumentException(SharpSvnStrings.RevisionTypeMustBeHeadDateOrSpecific, nameof(args));
            if (url.Revision.RequiresWorkingCopy)
                throw new ArgumentException(SharpSvnStrings.RevisionTypeMustBeHeadDateOrSpecific, nameof(url));

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            int version = 0;

            svn_opt_revision_t pegRev = url.Revision.AllocSvnRevision(pool);
            svn_opt_revision_t coRev = args.Revision.Or(url.Revision).Or(SvnRevision.Head).AllocSvnRevision(pool);

            svn_error_t r = svn_client.svn_client_checkout3(
                ref version,
                pool.AllocUri(url.Uri),
                pool.AllocDirent(path),
                pegRev,
                coRev,
                (svn_depth_t) args.Depth,
                args.IgnoreExternals,
                args.AllowObstructions,
                CtxHandle,
                pool.Handle);

            result = SvnUpdateResult.Create(this, args, version);

            return args.HandleResult(this, r, url);
        }
    }
}
