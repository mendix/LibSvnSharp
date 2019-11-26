using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    partial class SvnClient
    {
        /// <overloads>Create an unversioned copy of a tree (<c>svn export</c>)</overloads>
        /// <summary>Recursively exports the specified target to the specified path</summary>
        /// <remarks>Subversion optimizes this call if you specify a workingcopy file instead of an url</remarks>
        public bool Export(SvnTarget from, string toPath)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (toPath == null)
                throw new ArgumentNullException(nameof(toPath));
            if (!IsNotUri(toPath))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(toPath));

            return Export(from, toPath, new SvnExportArgs(), out _);
        }

        /// <summary>Recursively exports the specified target to the specified path</summary>
        /// <remarks>Subversion optimizes this call if you specify a workingcopy file instead of an url</remarks>
        public bool Export(SvnTarget from, string toPath, out SvnUpdateResult result)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (toPath == null)
                throw new ArgumentNullException(nameof(toPath));
            if (!IsNotUri(toPath))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(toPath));

            return Export(from, toPath, new SvnExportArgs(), out result);
        }

        /// <summary>Exports the specified target to the specified path</summary>
        /// <remarks>Subversion optimizes this call if you specify a workingcopy file instead of an url</remarks>
        public bool Export(SvnTarget from, string toPath, SvnExportArgs args)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (toPath == null)
                throw new ArgumentNullException(nameof(toPath));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Export(from, toPath, args, out _);
        }

        /// <summary>Exports the specified target to the specified path</summary>
        /// <remarks>Subversion optimizes this call if you specify a workingcopy file instead of an url</remarks>
        public unsafe bool Export(SvnTarget from, string toPath, SvnExportArgs args, out SvnUpdateResult result)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (toPath == null)
                throw new ArgumentNullException(nameof(toPath));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            EnsureState(SvnContextState.AuthorizationInitialized);

            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);

            result = null;

            int resultRev = 0;
            svn_opt_revision_t pegRev = from.Revision.AllocSvnRevision(pool);
            svn_opt_revision_t rev = args.Revision.Or(from.GetSvnRevision(SvnRevision.Working, SvnRevision.Head)).AllocSvnRevision(pool);

            svn_error_t r = svn_client.svn_client_export5(
                ref resultRev,
                from.AllocAsString(pool),
                pool.AllocDirent(toPath),
                pegRev,
                rev,
                args.Overwrite,
                args.IgnoreExternals,
                args.IgnoreKeywords,
                (svn_depth_t) args.Depth,
                pool.AllocString(GetEolPtr(args.LineStyle)),
                CtxHandle,
                pool.Handle);

            if (args.HandleResult(this, r, from))
            {
                result = SvnUpdateResult.Create(this, args, resultRev);
                return true;
            }

            return false;
        }

        internal static string GetEolPtr(SvnLineStyle style)
        {
            switch (style)
            {
                case SvnLineStyle.Default:
                case SvnLineStyle.Native:
                    return null;
                case SvnLineStyle.CarriageReturnLinefeed:
                    return "CRLF";
                case SvnLineStyle.Linefeed:
                    return "LF";
                case SvnLineStyle.CarriageReturn:
                    return "CR";
                default:
                    throw new ArgumentOutOfRangeException(nameof(style));
            }
        }
    }
}
