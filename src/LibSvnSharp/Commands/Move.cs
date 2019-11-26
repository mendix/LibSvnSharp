using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Move and/or rename something in working copy, remembering history (<c>svn move</c>)</overloads>
        /// <summary>Move and/or rename something in working copy, remembering history (<c>svn move</c>)</summary>
        public bool Move(string sourcePath, string toPath)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentNullException(nameof(sourcePath));
            if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException(nameof(toPath));

            return Move(NewSingleItemCollection(sourcePath), toPath, new SvnMoveArgs());
        }

        /// <summary>Move and/or rename something in working copy, remembering history (<c>svn move</c>)</summary>
        public bool Move(ICollection<string> sourcePaths, string toPath)
        {
            if (sourcePaths == null)
                throw new ArgumentNullException(nameof(sourcePaths));
            if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException(nameof(toPath));

            return Move(sourcePaths, toPath, new SvnMoveArgs());
        }

        /// <summary>Move and/or rename something in working copy, remembering history (<c>svn move</c>)</summary>
        public bool Move(string sourcePath, string toPath, SvnMoveArgs args)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentNullException(nameof(sourcePath));
            if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException(nameof(toPath));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Move(NewSingleItemCollection(sourcePath), toPath, args);
        }

        /// <summary>Move and/or rename something in working copy, remembering history (<c>svn move</c>)</summary>
        public unsafe bool Move(ICollection<string> sourcePaths, string toPath, SvnMoveArgs args)
        {
            if (sourcePaths == null)
                throw new ArgumentNullException(nameof(sourcePaths));
            if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException(nameof(toPath));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            foreach (string s in sourcePaths)
            {
                if (string.IsNullOrEmpty(s))
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(sourcePaths));
                if (!IsNotUri(s))
                    throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(sourcePaths));
            }

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            svn_error_t r = svn_client.svn_client_move7(
                AllocDirentArray(sourcePaths, pool),
                pool.AllocDirent(toPath),
                args.AlwaysMoveAsChild || (sourcePaths.Count > 1),
                args.CreateParents,
                args.AllowMixedRevisions,
                args.MetaDataOnly,
                null,
                null,
                IntPtr.Zero,
                CtxHandle,
                pool.Handle);

            return args.HandleResult(this, r, sourcePaths);
        }

        /// <overloads>Move and/or rename something in repository, remembering history (<c>svn move</c>)</overloads>
        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public bool RemoteMove(Uri sourceUri, Uri toUri)
        {
            if (sourceUri == null)
                throw new ArgumentNullException(nameof(sourceUri));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (!IsValidReposUri(sourceUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(sourceUri));
            if (!IsValidReposUri(toUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(toUri));

            return RemoteMove(NewSingleItemCollection(sourceUri), toUri, new SvnMoveArgs(), out _);
        }

        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public bool RemoteMove(ICollection<Uri> sourceUris, Uri toUri)
        {
            if (sourceUris == null)
                throw new ArgumentNullException(nameof(sourceUris));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (!IsValidReposUri(toUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(toUri));

            return RemoteMove(sourceUris, toUri, new SvnMoveArgs(), out _);
        }

        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public bool RemoteMove(Uri sourceUri, Uri toUri, out SvnCommitResult result)
        {
            if (sourceUri == null)
                throw new ArgumentNullException(nameof(sourceUri));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (!IsValidReposUri(sourceUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(sourceUri));
            if (!IsValidReposUri(toUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(toUri));

            return RemoteMove(NewSingleItemCollection(sourceUri), toUri, new SvnMoveArgs(), out result);
        }

        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public bool RemoteMove(ICollection<Uri> sourceUris, Uri toUri, out SvnCommitResult result)
        {
            if (sourceUris == null)
                throw new ArgumentNullException(nameof(sourceUris));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));

            return RemoteMove(sourceUris, toUri, new SvnMoveArgs(), out result);
        }

        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public bool RemoteMove(Uri sourceUri, Uri toUri, SvnMoveArgs args)
        {
            if (sourceUri == null)
                throw new ArgumentNullException(nameof(sourceUri));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return RemoteMove(NewSingleItemCollection(sourceUri), toUri, args, out _);
        }

        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public bool RemoteMove(ICollection<Uri> sourceUris, Uri toUri, SvnMoveArgs args)
        {
            if (sourceUris == null)
                throw new ArgumentNullException(nameof(sourceUris));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return RemoteMove(sourceUris, toUri, args, out _);
        }

        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public bool RemoteMove(Uri sourceUri, Uri toUri, SvnMoveArgs args, out SvnCommitResult result)
        {
            if (sourceUri == null)
                throw new ArgumentNullException(nameof(sourceUri));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return RemoteMove(NewSingleItemCollection(sourceUri), toUri, args, out result);
        }

        /// <summary>Move and/or rename something in repository, remembering history (<c>svn move</c>)</summary>
        public unsafe bool RemoteMove(ICollection<Uri> sourceUris, Uri toUri, SvnMoveArgs args, out SvnCommitResult result)
        {
            if (sourceUris == null)
                throw new ArgumentNullException(nameof(sourceUris));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (!IsValidReposUri(toUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(toUri));

            var uris = new List<string>(sourceUris.Count);

            foreach (Uri u in sourceUris)
            {
                if (u == null)
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(sourceUris));
                if (!IsValidReposUri(u))
                    throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(sourceUris));

                uris.Add(UriToCanonicalString(u));
            }

            EnsureState(SvnContextState.AuthorizationInitialized);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);
            using var crr = new CommitResultReceiver(this);

            svn_error_t r = svn_client.svn_client_move7(
                AllocArray(uris, pool),
                pool.AllocUri(toUri),
                args.AlwaysMoveAsChild || (sourceUris.Count > 1),
                args.CreateParents,
                args.AllowMixedRevisions,
                args.MetaDataOnly,
                CreateRevPropList(args.LogProperties, pool),
                crr.CommitCallback.Get(),
                crr.CommitBaton,
                CtxHandle,
                pool.Handle);

            result = crr.CommitResult;

            return args.HandleResult(this, r, sourceUris);
        }
    }
}
