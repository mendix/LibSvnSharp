using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LibSvnSharp.Implementation
{
    public sealed class SvnCommitItemCollection : KeyedCollection<string, SvnCommitItem>
    {
        internal SvnCommitItemCollection(IList<SvnCommitItem> items)
        {
            foreach (SvnCommitItem i in items)
            {
                Add(i);
            }
        }

        protected override string GetKeyForItem(SvnCommitItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return item.Path;
        }
    }
}
