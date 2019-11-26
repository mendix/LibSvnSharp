using System;
using System.Collections.ObjectModel;

namespace LibSvnSharp.Implementation
{
    public sealed class SvnChangeItemCollection : KeyedCollection<string, SvnChangeItem>
    {
        internal SvnChangeItemCollection()
        { }

        protected override string GetKeyForItem(SvnChangeItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return item.Path;
        }
    }
}
