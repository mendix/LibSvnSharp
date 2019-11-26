using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnChangeItem
    {
        svn_log_changed_path2_t _changed_path;
        string _copyFromPath;
        Uri _repositoryPath;

        internal unsafe SvnChangeItem(string path, svn_log_changed_path2_t changed_path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (changed_path == null)
                throw new ArgumentNullException(nameof(changed_path));

            _changed_path = changed_path;
            Path = path;
            Action = (SvnChangeAction)changed_path.action;
            _copyFromPath = SvnBase.Utf8_PtrToString(changed_path.copyfrom_path);
            CopyFromRevision = changed_path.copyfrom_path != null ? changed_path.copyfrom_rev : -1;
            NodeKind = (SvnNodeKind)changed_path.node_kind;

            switch (changed_path.text_modified)
            {
                case svn_tristate_t.svn_tristate_false:
                    ContentModified = false;
                    break;
                case svn_tristate_t.svn_tristate_true:
                    ContentModified = true;
                    break;
            }

            switch (changed_path.props_modified)
            {
                case svn_tristate_t.svn_tristate_false:
                    PropertiesModified = false;
                    break;
                case svn_tristate_t.svn_tristate_true:
                    PropertiesModified = true;
                    break;
            }
        }

        /// <summary>Gets the path inside rooted at the repository root (including initial '/')</summary>
        public string Path { get; }

        /// <summary>Gets the relative uri of the path inside the repository</summary>
        /// <remarks>Does not include an initial '/'. Ends with a '/' if <see cref="NodeKind" /> is <see cref="SvnNodeKind.Directory" />.</remarks>
        public Uri RepositoryPath
        {
            get
            {
                if (_repositoryPath == null && Path != null)
                    Uri.TryCreate(Path.Substring(1) + ((NodeKind == SvnNodeKind.Directory) ? "/" : ""), UriKind.Relative, out _repositoryPath);

                return _repositoryPath;
            }
        }

        public SvnChangeAction Action { get; }

        public unsafe string CopyFromPath
        {
            get
            {
                if (_copyFromPath == null && _changed_path != null && _changed_path.copyfrom_path != null)
                    _copyFromPath = SvnBase.Utf8_PtrToString(_changed_path.copyfrom_path);

                return _copyFromPath;
            }
        }

        public long CopyFromRevision { get; }

        /// <summary>Gets the node kind of the changed path (Only available when committed to a 1.6+ repository)</summary>
        public SvnNodeKind NodeKind { get; }

        /// <summary>Gets a boolean indicating whether the content of a node is modified in this revision. (Value only available for 1.7+ servers)</summary>
        public bool? ContentModified { get; }

        /// <summary>Gets a boolean indicating whether the versioned properties of a node are modified in this revision. (Value only available for 1.7+ servers)</summary>
        public bool? PropertiesModified { get; }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return CopyFromRevision.GetHashCode() ^ Path.GetHashCode();
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
                    GC.KeepAlive(CopyFromPath);
                }
            }
            finally
            {
                _changed_path = null;
            }
        }
    }
}
