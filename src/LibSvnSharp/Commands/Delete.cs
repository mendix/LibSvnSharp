using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Remove files and directories from version control, scheduling (<c>svn delete|remove</c>)</overloads>
        /// <summary>Remove files and directories from version control, scheduling (<c>svn delete|remove</c>)</summary>
        public bool Delete(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return Delete(NewSingleItemCollection(path), new SvnDeleteArgs());
        }

        /// <summary>Remove files and directories from version control, scheduling (<c>svn delete|remove</c>)</summary>
        public bool Delete(string path, SvnDeleteArgs args)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            return Delete(NewSingleItemCollection(path), args);
        }

        /// <summary>Remove files and directories from version control, scheduling (<c>svn delete|remove</c>)</summary>
        public bool Delete(ICollection<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            return Delete(paths, new SvnDeleteArgs());
        }

        /// <summary>Remove files and directories from version control, scheduling (<c>svn delete|remove</c>)</summary>
        public bool Delete(ICollection<string> paths, SvnDeleteArgs args)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(paths));
                if (!IsNotUri(path))
                    throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(paths));
            }

            EnsureState(SvnContextState.ConfigLoaded);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            var aprPaths = new AprArray<string, AprCStrDirentMarshaller>(paths, pool);

            svn_error_t r = svn_client.svn_client_delete4(
                aprPaths.Handle,
                args.Force,
                args.KeepLocal,
                null,
                null,
                IntPtr.Zero,
                CtxHandle,
                pool.Handle);

            return args.HandleResult(this, r, paths);
        }

        /// <overloads>Remove files and directories from version control at the repository (<c>svn delete|remove</c>)</overloads>
        /// <summary>Remove files and directories from version control at the repository (<c>svn delete|remove</c>)</summary>
        public bool RemoteDelete(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uri));

            return RemoteDelete(NewSingleItemCollection(uri), new SvnDeleteArgs());
        }

        /// <summary>Remove files and directories from version control at the repository (<c>svn delete|remove</c>)</summary>
        public bool RemoteDelete(Uri uri, SvnDeleteArgs args)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uri));

            return RemoteDelete(NewSingleItemCollection(uri), args);
        }

        /// <summary>Remove files and directories from version control at the repository (<c>svn delete|remove</c>)</summary>
        public bool RemoteDelete(Uri uri, SvnDeleteArgs args, out SvnCommitResult result)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uri));

            return RemoteDelete(NewSingleItemCollection(uri), args, out result);
        }

        /// <summary>Remove files and directories from version control at the repository (<c>svn delete|remove</c>)</summary>
        public bool RemoteDelete(ICollection<Uri> uris)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));

            return RemoteDelete(uris, new SvnDeleteArgs(), out _);
        }

        /// <summary>Remove files and directories from version control at the repository (<c>svn delete|remove</c>)</summary>
        public bool RemoteDelete(ICollection<Uri> uris, SvnDeleteArgs args)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return RemoteDelete(uris, args, out _);
        }

        /// <summary>Remove files and directories from version control at the repository (<c>svn delete|remove</c>)</summary>
        public bool RemoteDelete(ICollection<Uri> uris, SvnDeleteArgs args, out SvnCommitResult result)
        {
            if (uris == null)
                throw new ArgumentNullException(nameof(uris));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            result = null;

            var uriData = new string[uris.Count];
            int i = 0;

            foreach (Uri uri in uris)
            {
                if (uri == null)
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(uris));
                if (!IsValidReposUri(uri))
                    throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uris));

                uriData[i++] = UriToCanonicalString(uri);
            }

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);
            using var crr = new CommitResultReceiver(this);

            var aprPaths = new AprArray<string, AprUriMarshaller>(uriData, pool);

            svn_error_t r = svn_client.svn_client_delete4(
                aprPaths.Handle,
                args.Force,
                args.KeepLocal,
                CreateRevPropList(args.LogProperties, pool),
                crr.CommitCallback.Get(),
                crr.CommitBaton,
                CtxHandle,
                pool.Handle);

            result = crr.CommitResult;

            return args.HandleResult(this, r, uris);
        }
    }
}
