using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public sealed class SvnDirEntry
    {
        svn_dirent_t _entry;
        string _author;

        internal SvnDirEntry(svn_dirent_t entry)
        {
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
            NodeKind = (SvnNodeKind) entry.kind;
            FileSize = entry.size;
            HasProperties = entry.has_props;
            Revision = entry.created_rev;
            Time = (entry.time != 0) ? SvnBase.DateTimeFromAprTime(entry.time) : DateTime.MinValue;
        }

        public SvnNodeKind NodeKind { get; }

        /// <summary>Gets the length of the file text or 0 for directories</summary>
        public long FileSize { get; }

        /// <summary>Gets a boolean indicating whether this node has svn properties</summary>
        public bool HasProperties { get; }

        /// <summary>Gets the last revision in which this node changed</summary>
        public long Revision { get; }

        /// <summary>Gets the time of the last change</summary>
        public DateTime Time { get; }

        /// <summary>Gets the author of the last revision of this file</summary>
        public unsafe string Author
        {
            get
            {
                if (_author == null && _entry != null && _entry.last_author != null)
                    _author = SvnBase.Utf8_PtrToString(_entry.last_author);

                return _author;
            }
        }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return Time.GetHashCode() ^ SvnEventArgs.SafeGetHashCode(Author);
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
                    GC.KeepAlive(Author);
                }

            }
            finally
            {
                _entry = null;

                //base.Detach(keepProperties);
            }
        }
    }
}
