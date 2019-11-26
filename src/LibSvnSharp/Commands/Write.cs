using System;
using System.IO;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Writes the content of specified files or URLs to a stream. (<c>svn cat</c>)</overloads>
        /// <summary>Writes the content of specified files or URLs to a stream. (<c>svn cat</c>)</summary>
        public bool Write(SvnTarget target, Stream output)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            return Write(target, output, new SvnWriteArgs());
        }

        /// <summary>Writes the content of specified files or URLs to a stream. (<c>svn cat</c>)</summary>
        public unsafe bool Write(SvnTarget target, Stream output, SvnWriteArgs args)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (args == null)
                throw new ObjectDisposedException(nameof(args));

            using var pool = new AprPool(_pool);

            return InternalWrite(target, output, args, null, pool);
        }

        /// <summary>Writes the content of specified files or URLs to a stream. (<c>svn cat</c>)</summary>
        public bool Write(SvnTarget target, Stream output, out SvnPropertyCollection properties)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            return Write(target, output, new SvnWriteArgs(), out properties);
        }

        /// <summary>Writes the content of specified files or URLs to a stream. (<c>svn cat</c>)</summary>
        public unsafe bool Write(SvnTarget target, Stream output, SvnWriteArgs args, out SvnPropertyCollection properties)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (args == null)
                throw new ObjectDisposedException(nameof(args));

            using var pool = new AprPool(_pool);
            apr_hash_t.__Internal* props_ptr = null;

            properties = null;

            if (InternalWrite(target, output, args, &props_ptr, pool))
            {
                var props = apr_hash_t.__CreateInstance(new IntPtr(props_ptr));
                properties = CreatePropertyDictionary(props, pool);
                return true;
            }

            return false;
        }

        unsafe bool InternalWrite(SvnTarget target, Stream output, SvnWriteArgs args, apr_hash_t.__Internal** props, AprPool resultPool)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (args == null)
                throw new ObjectDisposedException(nameof(args));

            using var scratchPool = new AprPool(resultPool);
            EnsureState(SvnContextState.AuthorizationInitialized);
            using var store = new ArgsStore(this, args, scratchPool);

            using var wrapper = new SvnStreamWrapper(output, false, true, scratchPool);

            svn_opt_revision_t pegRev = target.Revision.AllocSvnRevision(scratchPool);
            svn_opt_revision_t rev = args.Revision.Or(target.Revision).AllocSvnRevision(scratchPool);

            svn_error_t r = svn_client.svn_client_cat3(
                (void**) props,
                wrapper.Handle,
                target.AllocAsString(scratchPool, true),
                pegRev,
                rev,
                !args.IgnoreKeywords,
                CtxHandle,
                resultPool.Handle,
                scratchPool.Handle);

            return args.HandleResult(this, r, target);
        }
    }
}
