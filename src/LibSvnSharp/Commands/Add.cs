using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Put files and directories under version control, scheduling them for addition to repository.
        /// They will be added in next commit(<c>svn add</c>)</overloads>
        /// <summary>Recursively adds the specified path</summary>
        /// <exception type="SvnException">Operation failed</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Add(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return Add(path, new SvnAddArgs());
        }

        /// <summary>Adds the specified path</summary>
        /// <exception type="SvnException">Operation failed</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public bool Add(string path, SvnDepth depth)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var args = new SvnAddArgs();

            args.Depth = depth;

            return Add(path, args);
        }

        /// <summary>Adds the specified path</summary>
        /// <returns>true if the operation succeeded; false if it did not</returns>
        /// <exception type="SvnException">Operation failed and args.ThrowOnError = true</exception>
        /// <exception type="ArgumentException">Parameters invalid</exception>
        public unsafe bool Add(string path, SvnAddArgs args)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            EnsureState(SvnContextState.ConfigLoaded, SvnExtendedState.MimeTypesLoaded);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            svn_error_t r = svn_client.svn_client_add5(
                pool.AllocDirent(path),
                (svn_depth_t) args.Depth,
                args.Force,
                args.NoIgnore,
                args.NoAutoProps,
                args.AddParents,
                CtxHandle,
                pool.Handle);

            return args.HandleResult(this, r, path);
        }
    }
}
