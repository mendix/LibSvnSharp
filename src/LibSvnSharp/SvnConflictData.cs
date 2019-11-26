using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnConflictData
    {
        svn_wc_conflict_description2_t _description;
        AprPool _pool;

        string _propertyName;
        string _fullPath;
        string _path;
        string _mimeType;

        string _baseFile;
        string _theirFile;
        string _myFile;
        string _mergedFile;
        string _prejFile;

        SvnConflictSource _leftOrigin;
        SvnConflictSource _rightOrigin;

        internal SvnConflictData(svn_wc_conflict_description2_t description, AprPool pool)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _description = description;
            _pool = pool;

            IsBinary = description.is_binary;
            NodeKind = (SvnNodeKind)description.node_kind;
            ConflictAction = (SvnConflictAction)description.action;
            ConflictReason = (SvnConflictReason)description.reason;
            ConflictType = (SvnConflictType)description.kind;
            Operation = (SvnOperation)description.operation;
        }

        public unsafe string Name
        {
            get
            {
                if (_path == null && _description != null && _description.local_abspath != null && _pool != null)
                    _path = SvnBase.Utf8_PathPtrToString(svn_dirent_uri.svn_dirent_basename(_description.local_abspath, null), _pool);

                return _path;
            }
        }

        public unsafe string FullPath
        {
            get
            {
                if (_fullPath != null && _description != null && _description.local_abspath != null && _pool != null)
                    _fullPath = SvnBase.Utf8_PathPtrToString(_description.local_abspath, _pool);

                return _fullPath;
            }
        }

        public unsafe string PropertyName
        {
            get
            {
                if (_propertyName == null && _description != null && _description.property_name != null)
                    _propertyName = SvnBase.Utf8_PtrToString(_description.property_name);

                return _propertyName;
            }
        }

        public unsafe string MimeType
        {
            get
            {
                if (_mimeType == null && _description != null && _description.mime_type != null)
                    _mimeType = SvnBase.Utf8_PtrToString(_description.mime_type);

                return _mimeType;
            }
        }

        public unsafe string BaseFile
        {
            get
            {
                if (_baseFile == null && _description != null && _description.base_abspath != null && _pool != null)
                    _baseFile = SvnBase.Utf8_PathPtrToString(_description.base_abspath, _pool);

                return _baseFile;
            }
        }

        public unsafe string TheirFile
        {
            get
            {
                if (_theirFile == null && _description != null && _description.their_abspath != null && _pool != null)
                    _theirFile = SvnBase.Utf8_PathPtrToString(_description.their_abspath, _pool);

                return _theirFile;
            }
        }

        public unsafe string MyFile
        {
            get
            {
                if (_myFile == null && _description != null && _description.my_abspath != null && _pool != null)
                    _myFile = SvnBase.Utf8_PathPtrToString(_description.my_abspath, _pool);

                return _myFile;
            }
        }

        public unsafe string MergedFile
        {
            get
            {
                if (_mergedFile == null && _description != null && _description.merged_file != null && _pool != null)
                    _mergedFile  = SvnBase.Utf8_PathPtrToString(_description.merged_file, _pool);

                return _mergedFile;
            }
        }

        public unsafe string PropertyRejectFile
        {
            get
            {
                if (_prejFile == null && _description != null && _description.prop_reject_abspath != null && _pool != null)
                    _prejFile = SvnBase.Utf8_PathPtrToString(_description.prop_reject_abspath, _pool);

                return _prejFile;
            }
        }

        public bool IsBinary { get; }

        public SvnConflictAction ConflictAction { get; }

        public SvnConflictReason ConflictReason { get; }

        public SvnConflictType ConflictType { get; }

        public SvnNodeKind NodeKind { get; }

        /// <summary>Gets the operation creating the tree conflict</summary>
        public SvnOperation Operation { get; }

        /* ### TODO:
        const svn_string_t *prop_value_base;
        const svn_string_t *prop_value_working;
        const svn_string_t *prop_value_incoming_old;
        const svn_string_t *prop_value_incoming_new;*/

        public SvnConflictSource LeftSource
        {
            get
            {
                if (_leftOrigin == null && _description != null && _description.src_left_version != null && _pool != null)
                    _leftOrigin = new SvnConflictSource(_description.src_left_version, _pool);

                return _leftOrigin;
            }
        }

        public SvnConflictSource RightSource
        {
            get
            {
                if (_rightOrigin == null && _description != null && _description.src_right_version != null && _pool != null)
                    _rightOrigin = new SvnConflictSource(_description.src_right_version, _pool);

                return _rightOrigin;
            }
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
                    GC.KeepAlive(Name);
                    GC.KeepAlive(FullPath);
                    GC.KeepAlive(PropertyName);
                    GC.KeepAlive(MimeType);
                    GC.KeepAlive(BaseFile);
                    GC.KeepAlive(TheirFile);
                    GC.KeepAlive(MyFile);
                    GC.KeepAlive(MergedFile);
                    GC.KeepAlive(PropertyRejectFile);

                    GC.KeepAlive(LeftSource);
                    GC.KeepAlive(RightSource);
                }

                _leftOrigin?.Detach(keepProperties);
                _rightOrigin?.Detach(keepProperties);
            }
            finally
            {
                _description = null;
                _pool = null;
            }
        }
    }
}
