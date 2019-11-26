using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnConflictEventArgs : SvnCancelEventArgs
    {
        svn_wc_conflict_description2_t _description;
        AprPool _pool;
        SvnAccept _result;

        SvnConflictData _data;
        string _mergeFile;

        // BH: Note svn_wc_conflict_description_t is also mapped by SvnConflictData
        // for the non-event case
        internal SvnConflictEventArgs(svn_wc_conflict_description2_t description, AprPool pool)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _description = description;
            _pool = pool;
            _result = SvnAccept.Postpone;
        }

        public SvnConflictData Conflict
        {
            get
            {
                if (_data == null && _description != null && _pool != null)
                    _data = new SvnConflictData(_description, _pool);

                return _data;
            }
        }

        public SvnAccept Choice
        {
            get => _result;
            set => _result = EnumVerifier.Verify(value);
        }

        /// <summary><see cref="SvnConflictData.Name" /></summary>
        public string Path => Conflict?.Name;

        /// <summary><see cref="SvnConflictData.PropertyName" /></summary>
        public string PropertyName => Conflict?.PropertyName;

        /// <summary><see cref="SvnConflictData.MimeType" /></summary>
        public string MimeType => Conflict?.MimeType;

        public string BaseFile => Conflict?.BaseFile;

        public string TheirFile => Conflict?.TheirFile;

        public string MyFile => Conflict?.MyFile;

        public string MergedFile
        {
            get
            {
                if (_mergeFile != null)
                    return _mergeFile;

                return Conflict?.MergedFile;
            }
            set
            {
                if (string.IsNullOrEmpty(value) && (value != MergedFile))
                    throw new InvalidOperationException("Only settable with valid filename");

                _mergeFile = value;
            }
        }

        public string MergedValue { get; set; }

        public bool IsBinary => Conflict != null && Conflict.IsBinary;

        public SvnConflictAction ConflictAction => Conflict?.ConflictAction ?? 0;

        public SvnConflictReason ConflictReason => Conflict?.ConflictReason ?? 0;

        public SvnConflictType ConflictType => Conflict?.ConflictType ?? 0;

        public SvnNodeKind NodeKind => Conflict?.NodeKind ?? SvnNodeKind.None;

        protected internal override void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    GC.KeepAlive(Conflict);
                }

                _data?.Detach(keepProperties);
            }
            finally
            {
                _pool = null;
                _description = null;
                base.Detach(keepProperties);
            }
        }
    }
}
