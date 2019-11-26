using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Send changes from your working copy to the repository (<c>svn commit</c>)</overloads>
        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return Commit(NewSingleItemCollection(path), new SvnCommitArgs(), out _);
        }

        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(string path, out SvnCommitResult result)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return Commit(NewSingleItemCollection(path), new SvnCommitArgs(), out result);
        }

        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(ICollection<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            return Commit(paths, new SvnCommitArgs(), out _);
        }

        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(ICollection<string> paths, out SvnCommitResult result)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            return Commit(paths, new SvnCommitArgs(), out result);
        }

        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(string path, SvnCommitArgs args)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Commit(NewSingleItemCollection(path), args, out _);
        }

        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(string path, SvnCommitArgs args, out SvnCommitResult result)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Commit(NewSingleItemCollection(path), args, out result);
        }

        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(ICollection<string> paths, SvnCommitArgs args)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Commit(paths, args, out _);
        }

        /// <summary>Send changes from your working copy to the repository (<c>svn commit</c>)</summary>
        public bool Commit(ICollection<string> paths, SvnCommitArgs args, out SvnCommitResult result)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (paths.Count == 0)
                throw new ArgumentException(SharpSvnStrings.CollectionMustContainAtLeastOneItem, nameof(paths));

            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(paths));
                if (!IsNotUri(path))
                    throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(paths));
            }

            EnsureState(SvnContextState.AuthorizationInitialized, args.RunTortoiseHooks ? SvnExtendedState.TortoiseSvnHooksLoaded : SvnExtendedState.None);
            using var pool = new AprPool(_pool);
            using var store = new ArgsStore(this, args, pool);
            using var crr = new CommitResultReceiver(this);

            var aprPaths = new AprArray<string, AprCStrDirentMarshaller>(paths, pool);

#if TORTOISE_HOOKS_AVAILABLE
            string commonPath = null;
            string pathsFile = null;
            string msgFile = null;
            SvnClientHook preCommitHook = null;
            SvnClientHook postCommitHook = null;

            if (args.RunTortoiseHooks)
            {
                const char *pCommonPath;
                SVN_HANDLE(svn_dirent_condense_targets(&pCommonPath, NULL, aprPaths->Handle, FALSE, pool.Handle, pool.Handle));
                if (pCommonPath && pCommonPath[0] != '\0')
                {
                    commonPath = Utf8_PathPtrToString(pCommonPath, %pool);
                }

                if (!String::IsNullOrEmpty(commonPath))
                {
                    FindHook(commonPath, SvnClientHookType::PreCommit, preCommitHook);
                    FindHook(commonPath, SvnClientHookType::PostCommit, postCommitHook);
                }

                if (preCommitHook || postCommitHook)
                {
                    AprPool subpool(%pool);
                    const char *path;
                    svn_stream_t *f;
                    const apr_array_header_t *h = aprPaths->Handle;

                    /* Delete the tempfile on disposing the SvnClient */
                    SVN_HANDLE(svn_stream_open_unique(&f, &path, null, svn_io_file_del_on_pool_cleanup,
                                                                                        _pool.Handle, subpool.Handle));

                    for (int i = 0; i < h->nelts; i++)
                    {
                        SVN_HANDLE(svn_stream_printf(f, subpool.Handle, "%s\n",
                                                                                    svn_dirent_local_style(APR_ARRAY_IDX(h, i, const char *), subpool.Handle)));
                    }
                    SVN_HANDLE(svn_stream_close(f));
                    pathsFile = Utf8_PathPtrToString(path, %subpool);

                    /* Delete the tempfile on disposing the SvnClient */
                    SVN_HANDLE(svn_stream_open_unique(&f, &path, null, svn_io_file_del_on_pool_cleanup,
                                                                                        _pool.Handle, subpool.Handle));

                    SVN_HANDLE(svn_stream_printf(f, subpool.Handle, "%s",
                                                                                subpool.AllocString(args->LogMessage)));
                    SVN_HANDLE(svn_stream_close(f));

                    msgFile = Utf8_PathPtrToString(path, %subpool);
                }
            }

            if (preCommitHook != null)
            {
                if (!preCommitHook->Run(this, args,
                                        pathsFile,
                                        ((int)args->Depth).ToString(CultureInfo::InvariantCulture),
                                        msgFile,
                                        commonPath))
                {
                    return args->HandleResult(this, new SvnClientHookException("TortoiseSVN Client hook 'pre-commit' rejected commit"));
                }

                // Read the log message back from the hook script
                AprPool subpool(%pool);
                svn_stream_t *f;
                svn_string_t *msg;

                SVN_HANDLE(svn_stream_open_readonly(&f, subpool.AllocDirent(msgFile), subpool.Handle, subpool.Handle));
                SVN_HANDLE(svn_string_from_stream(&msg, f, subpool.Handle, subpool.Handle));
                SVN_HANDLE(svn_stream_close(f));

                // Overwrite the previous log message with the (possibly) adjusted one from the hook script
                args->LogMessage = SvnBase::Utf8_PtrToString(msg->data, msg->len);
            }
#endif // TORTOISE_HOOKS_AVAILABLE

            svn_error_t r = svn_client.svn_client_commit6(
                aprPaths.Handle,
                (svn_depth_t) args.Depth,
                args.KeepLocks,
                args.KeepChangeLists,
                true,
                args.IncludeFileExternals,
                args.IncludeDirectoryExternals,
                CreateChangeListsList(args.ChangeLists, pool), // Intersect ChangeLists
                CreateRevPropList(args.LogProperties, pool),
                crr.CommitCallback.Get(),
                crr.CommitBaton,
                CtxHandle,
                pool.Handle);

            result = crr.CommitResult;

#if TORTOISE_HOOKS_AVAILABLE
            if (postCommitHook != null)
            {
                AprPool subpool(%pool);
                const char *path;
                svn_stream_t *f;
                char *tmpBuf = (char*)subpool.Alloc(1024);

                /* Delete the tempfile on disposing the SvnClient */
                SVN_HANDLE(svn_stream_open_unique(&f, &path, null, svn_io_file_del_on_pool_cleanup,
                                                    _pool.Handle, subpool.Handle));

                svn_error_t *rr = r;

                while(rr)
                {
                    SVN_HANDLE(svn_stream_printf(f, subpool.Handle, "%s\n",
                                                    svn_err_best_message(rr, tmpBuf, 1024)));

                    rr = rr->child;
                }

                SVN_HANDLE(svn_stream_close(f));
                String^ errFile = Utf8_PathPtrToString(path, %subpool);

                if (!postCommitHook->Run(this, args,
                                            pathsFile,
                                            ((int)args->Depth).ToString(CultureInfo::InvariantCulture),
                                            msgFile,
                                            (result ? result->Revision : -1).ToString(CultureInfo::InvariantCulture),
                                            errFile,
                                            commonPath))
                {
                    return args->HandleResult(this, new SvnClientHookException("TortoiseSVN Client hook 'post-commit' failed"));
                }
            }
#endif // TORTOISE_HOOKS_AVAILABLE

            return args.HandleResult(this, r, paths);
        }
    }
}
