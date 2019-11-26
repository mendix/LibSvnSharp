using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Interop.Svn.Delegates;

namespace LibSvnSharp
{
    public abstract class SvnLoggingEventArgs : SvnCancelEventArgs
    {
        svn_log_entry_t _entry;
        AprPool _pool;

        string _author;
        string _message;
        unsafe sbyte* _pcAuthor;
        unsafe sbyte* _pcMessage;
        SvnPropertyCollection _customProperties;

        SvnChangeItemCollection _changedPaths;
        SvnChangeItem[] _changeItemsToDetach;

        internal unsafe SvnLoggingEventArgs(svn_log_entry_t entry, AprPool pool)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _entry = entry;
            _pool = pool;

            sbyte* pcAuthor = null;
            sbyte* pcDate = null;
            sbyte* pcMessage = null;

            if (entry.revprops != null)
                svn_compat.svn_compat_log_revprops_out(&pcAuthor, &pcDate, &pcMessage, entry.revprops);

            if (pcDate != null)
            {
                long when = 0; // Documentation: date must be parsable by svn_time_from_cstring()
                svn_error_t err = pcDate != null ? svn_time.svn_time_from_cstring(ref when, pcDate, pool.Handle) : null;

                if (err == null)
                    Time = SvnBase.DateTimeFromAprTime(when);
                else
                    svn_error.svn_error_clear(err);
            }
            else
                Time = DateTime.MinValue;

            Revision = entry.revision;
            _pcAuthor = pcAuthor;
            _pcMessage = pcMessage;

            NotInheritable = entry.non_inheritable;
            SubtractiveMerge = entry.subtractive_merge;
        }

        public unsafe SvnChangeItemCollection ChangedPaths
        {
            get
            {
                if (_changedPaths == null && _entry != null && _entry.changed_paths2 != null && _pool != null)
                {
                    _changedPaths = new SvnChangeItemCollection();

                    /* Get an array of sorted hash keys. */
                    apr_array_header_t sorted_paths;

                    using (var comparisonFuncHandle = new SafeFuncHandle<Func_int_IntPtr_IntPtr>(
                        svn_sorts.__Internal.svn_sort_compare_items_as_paths))
                    {
                        sorted_paths = svn_sorts_private.svn_sort__hash(
                            _entry.changed_paths2, comparisonFuncHandle.Get(), _pool.Handle);
                    }

                    for (int i = 0; i < sorted_paths.nelts; i++)
                    {
                        var item = &(((svn_sort__item_t.__Internal*)(sorted_paths).elts)[i]);
                        sbyte* path = (sbyte*)item->key;
                        var pChangeInfo = item->value;

                        var ci = new SvnChangeItem(
                            SvnBase.Utf8_PtrToString(path),
                            svn_log_changed_path2_t.__CreateInstance(pChangeInfo));

                        _changedPaths.Add(ci);
                    }

                    if (_changedPaths.Count != 0)
                    {
                        _changeItemsToDetach = new SvnChangeItem[_changedPaths.Count];
                        _changedPaths.CopyTo(_changeItemsToDetach, 0);
                    }
                }

                return _changedPaths;
            }
        }

        /// <summary>Gets the list of custom properties retrieved with the log</summary>
        /// <remarks>Properties must be listed in SvnLogArgs.RetrieveProperties to be available here</remarks>
        public SvnPropertyCollection RevisionProperties
        {
            get
            {
                if (_customProperties == null && _entry?.revprops != null && _pool != null)
                {
                    _customProperties = SvnBase.CreatePropertyDictionary(_entry.revprops, _pool);
                }
                return _customProperties;
            }
        }

        [Obsolete("Use .RevisionProperties")]
        public SvnPropertyCollection CustomProperties => RevisionProperties;

        public long Revision { get; }

        public unsafe string Author
        {
            get
            {
                if (_author == null && _pcAuthor != null)
                    _author = SvnBase.Utf8_PtrToString(_pcAuthor);

                return _author;
            }
        }

        public DateTime Time { get; }

        public unsafe string LogMessage
        {
            get
            {
                if (_message == null && _pcMessage != null)
                {
                    _message = SvnBase.Utf8_PtrToString(_pcMessage);

                    if (_message != null)
                    {
                        // Subversion log messages always use \n newlines
                        _message = _message.Replace("\n", Environment.NewLine);
                    }
                }

                return _message;
            }
        }

        /// <summary>MergeInfo only: Not inheritable</summary>
        public bool NotInheritable { get; }

        /// <summary>MergeInfo only: Subtractive merge</summary>
        public bool SubtractiveMerge { get; }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode() 
        {
            return Revision.GetHashCode();
        }

        protected internal override unsafe void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    // Use all properties to get them cached in .Net memory
                    GC.KeepAlive(ChangedPaths);
                    GC.KeepAlive(Author);
                    GC.KeepAlive(LogMessage);
                    GC.KeepAlive(RevisionProperties);
                }

                if (_changeItemsToDetach != null)
                {
                    foreach (SvnChangeItem i in _changeItemsToDetach)
                    {
                        i.Detach(keepProperties);
                    }
                }
            }
            finally
            {
                _entry = null;
                _pool = null;
                _pcMessage = null;
                _pcAuthor = null;
                _changeItemsToDetach = null;
                base.Detach(keepProperties);
            }
        }
    }
}
