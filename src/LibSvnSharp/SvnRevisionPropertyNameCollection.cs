using System;
using System.Collections.ObjectModel;
using LibSvnSharp.Interop;

namespace LibSvnSharp
{
    public sealed class SvnRevisionPropertyNameCollection : KeyedCollection<string, string>
    {
        internal SvnRevisionPropertyNameCollection(bool initialEmpty)
            : base(StringComparer.Ordinal, 10) // Start using hashtable at 10 items
        {
            if (!initialEmpty)
            {
                Add(Constants.SVN_PROP_REVISION_AUTHOR);
                Add(Constants.SVN_PROP_REVISION_DATE);
                Add(Constants.SVN_PROP_REVISION_LOG);
            }
        }

        public void AddDefaultProperties()
        {
            if (!Contains(Constants.SVN_PROP_REVISION_AUTHOR))
                Add(Constants.SVN_PROP_REVISION_AUTHOR);

            if (!Contains(Constants.SVN_PROP_REVISION_DATE))
                Add(Constants.SVN_PROP_REVISION_DATE);

            if (!Contains(Constants.SVN_PROP_REVISION_LOG))
                Add(Constants.SVN_PROP_REVISION_LOG);
        }

        protected override string GetKeyForItem(string item)
        {
            return item;
        }

        public static readonly string Author = Constants.SVN_PROP_REVISION_AUTHOR;
        public static readonly string Date = Constants.SVN_PROP_REVISION_DATE;
        public static readonly string Log = Constants.SVN_PROP_REVISION_LOG;
    }
}
