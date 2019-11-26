using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    partial class SvnClient
    {
        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        public bool RemoteCopy(SvnTarget source, Uri toUri)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (!SvnBase.IsValidReposUri(toUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(toUri));

            return RemoteCopy(NewSingleItemCollection(source), toUri, new SvnCopyArgs(), out _);
        }

        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        /// <remarks>Can be called with either a list of <see cref="SvnTarget" />, <see cref="SvnUriTarget" /> or <see cref="SvnPathTarget" />.
        /// All members must be of the same type.</remarks>
        public bool RemoteCopy<TSvnTarget>(ICollection<TSvnTarget> sources, Uri toUri)
            where TSvnTarget : SvnTarget
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));

            return RemoteCopy(sources, toUri, new SvnCopyArgs(), out _);
        }

        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        public bool RemoteCopy(SvnTarget source, Uri toUri, out SvnCommitResult result)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));

            return RemoteCopy(NewSingleItemCollection(source), toUri, new SvnCopyArgs(), out result);
        }

        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        /// <remarks>Can be called with either a list of <see cref="SvnTarget" />, <see cref="SvnUriTarget" /> or <see cref="SvnPathTarget" />.
        /// All members must be of the same type.</remarks>
        public bool RemoteCopy<TSvnTarget>(ICollection<TSvnTarget> sources, Uri toUri, out SvnCommitResult result)
            where TSvnTarget : SvnTarget
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));

            return RemoteCopy(sources, toUri, new SvnCopyArgs(), out result);
        }

        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        public bool RemoteCopy(SvnTarget source, Uri toUri, SvnCopyArgs args)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return RemoteCopy(NewSingleItemCollection(source), toUri, args, out _);
        }

        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        /// <remarks>Can be called with either a list of <see cref="SvnTarget" />, <see cref="SvnUriTarget" /> or <see cref="SvnPathTarget" />.
        /// All members must be of the same type.</remarks>
        public bool RemoteCopy<TSvnTarget>(ICollection<TSvnTarget> sources, Uri toUri, SvnCopyArgs args)
            where TSvnTarget : SvnTarget
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return RemoteCopy(sources, toUri, args, out _);
        }

        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        public bool RemoteCopy(SvnTarget source, Uri toUri, SvnCopyArgs args, out SvnCommitResult result)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return RemoteCopy(NewSingleItemCollection(source), toUri, args, out result);
        }

        /// <summary>Duplicate something in repository, remembering history (<c>svn copy</c>)</summary>
        /// <remarks>Can be called with either a list of <see cref="SvnTarget" />, <see cref="SvnUriTarget" /> or <see cref="SvnPathTarget" />.
        /// All members must be of the same type.</remarks>
        public unsafe bool RemoteCopy<TSvnTarget>(ICollection<TSvnTarget> sources, Uri toUri, SvnCopyArgs args, out SvnCommitResult result)
            where TSvnTarget : SvnTarget
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (toUri == null)
                throw new ArgumentNullException(nameof(toUri));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (!SvnBase.IsValidReposUri(toUri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(toUri));
            if (sources.Count == 0)
                throw new ArgumentException(SharpSvnStrings.CollectionMustContainAtLeastOneItem, nameof(sources));

            bool isFirst = true;
            bool isLocal = false;

            foreach (SvnTarget target in sources)
            {
                if (target == null)
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(sources));

                SvnPathTarget pt = target as SvnPathTarget;
                if (isFirst)
                {
                    isLocal = (null != pt);
                    isFirst = false;
                }
                else if (isLocal != (null != pt))
                    throw new ArgumentException(SharpSvnStrings.AllTargetsMustBeUriOrPath, nameof(sources));
            }

            EnsureState(SvnContextState.AuthorizationInitialized);

            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);
            using var crr = new CommitResultReceiver(this);

            apr_array_header_t copies = AllocCopyArray(sources, pool);

            if (copies != null && args.Revision.RevisionType != SvnRevisionType.None)
            {
                svn_opt_revision_t rev = args.Revision.AllocSvnRevision(pool);

                for (int i = 0; i < copies.nelts; i++)
                {
                    var cp = ((svn_client_copy_source_t.__Internal**) copies.elts)[i];

                    cp->revision = rev.__Instance;
                }
            }

            svn_error_t r = svn_client.svn_client_copy7(
                copies,
                pool.AllocUri(toUri),
                args.AlwaysCopyAsChild || (sources.Count > 1),
                args.CreateParents,
                args.IgnoreExternals,
                args.MetaDataOnly,
                args.PinExternals,
                null /* */,
                CreateRevPropList(args.LogProperties, pool),
                crr.CommitCallback.Get(),
                crr.CommitBaton,
                CtxHandle,
                pool.Handle);

            result = crr.CommitResult;

            return args.HandleResult(this, r, sources);
        }
    }
}
