using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnCommitItem
    {
        svn_client_commit_item3_t _info;
        AprPool _pool;
        string _path;
        string _fullPath;
        Uri _uri;
        Uri _copyFromUri;
        string _movedFrom;

        internal SvnCommitItem(svn_client_commit_item3_t commitItemInfo, AprPool pool)
        {
            if (commitItemInfo == null)
                throw new ArgumentNullException(nameof(commitItemInfo));

            _info = commitItemInfo;
            NodeKind = (SvnNodeKind)commitItemInfo.kind;
            Revision = commitItemInfo.revision;
            CopyFromRevision = commitItemInfo.copyfrom_rev;
            CommitType = (SvnCommitTypes)commitItemInfo.state_flags;
            _pool = pool;
        }

        public unsafe string Path
        {
            get
            {
                if (_path == null && _info != null && _pool != null)
                    _path = SvnBase.Utf8_PathPtrToString(_info.path, _pool);
                return _path;
            }
        }

        public string FullPath
        {
            get
            {
                if (_fullPath == null && Path != null)
                    _fullPath = SvnTools.GetNormalizedFullPath(Path);

                return _fullPath;
            }
        }

        public unsafe string MovedFrom
        {
            get
            {
                if (_movedFrom == null && _info != null && _info.moved_from_abspath != null && _pool != null)
                    _movedFrom = SvnBase.Utf8_PathPtrToString(_info.moved_from_abspath, _pool);
                return _movedFrom;
            }
        }

        public SvnNodeKind NodeKind { get; }

        public unsafe Uri Uri
        {
            get
            {
                if (_uri == null && _info != null && _info.url != null)
                    _uri = SvnBase.Utf8_PtrToUri(_info.url, NodeKind);

                return _uri;
            }
        }

        public long Revision { get; }

        public unsafe Uri CopyFromUri
        {
            get
            {
                if (_copyFromUri == null && _info != null && _info.copyfrom_url != null)
                    _copyFromUri = SvnBase.Utf8_PtrToUri(_info.copyfrom_url, NodeKind);

                return _copyFromUri;
            }
        }

        public long CopyFromRevision { get; }

        public SvnCommitTypes CommitType { get; }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return Revision.GetHashCode();
        }

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
                    GC.KeepAlive(Path);
                    GC.KeepAlive(Uri);
                    GC.KeepAlive(CopyFromUri);
                    GC.KeepAlive(MovedFrom);
                }
            }
            finally
            {
                _info = null;
                _pool = null;
            }
        }
    }
}
