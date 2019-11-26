using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnConflictSource : SvnBase, ISvnOrigin
    {
        svn_wc_conflict_version_t _version;
        AprPool _pool;

        Uri _uri;
        Uri _repositoryRoot;
        Uri _repositoryPath;

        internal SvnConflictSource(svn_wc_conflict_version_t version, AprPool pool)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _version = version;
            _pool = pool;
            Revision = version.peg_rev;
            NodeKind = (SvnNodeKind)version.node_kind;
        }

        public unsafe Uri Uri
        {
            get
            {
                if (_uri == null && _version != null && _version.repos_url != null && _version.path_in_repos != null && _pool != null)
                    _uri = SvnBase.Utf8_PtrToUri(svn_path.svn_path_url_add_component2(_version.repos_url, _version.path_in_repos, _pool.Handle), NodeKind);

                return _uri;
            }
        }

        public unsafe Uri RepositoryRoot
        {
            get
            {
                if (_repositoryRoot == null && _version != null && _version.repos_url != null && _pool != null)
                    _repositoryRoot = SvnBase.Utf8_PtrToUri(_version.repos_url, SvnNodeKind.Directory);

                return _repositoryRoot;
            }
        }

        /// <summary>Gets the relative uri of the path inside the repository</summary>
        /// <remarks>Does not include an initial '/'. Ends with a '/' if <see cref="NodeKind" /> is <see cref="SvnNodeKind::Directory" />.</remarks>
        public Uri RepositoryPath
        {
            get
            {
                if (_repositoryPath == null && RepositoryRoot != null && Uri != null)
                    _repositoryPath = RepositoryRoot.MakeRelativeUri(Uri);

                return _repositoryPath;
            }
        }

        /// <summary>Gets the revision of <see cref="Uri" /></summary>
        public long Revision { get; }

        /// <summary>Gets the <see cref="SvnNodeKind" /> of <see cref="Uri" /></summary>
        public SvnNodeKind NodeKind { get; }

        public SvnUriTarget Target => new SvnUriTarget(Uri, Revision);

        public static implicit operator SvnUriTarget(SvnConflictSource value)
        {
            return value?.Target;
        }

        SvnTarget RawTarget => Target;

        SvnTarget ISvnOrigin.Target => RawTarget;

        public void Detach()
        {
            Detach(true);
        }
        
        internal void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    GC.KeepAlive(Uri);
                    GC.KeepAlive(RepositoryRoot);
                }
            }
            finally
            {
                _version = null;
                _pool = null;
            }
        }
    }
}
