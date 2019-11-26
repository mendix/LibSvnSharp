using System.Collections.ObjectModel;

namespace LibSvnSharp
{
    public sealed class SvnPropertyCollection : KeyedCollection<string, SvnPropertyValue>
    {
        protected override string GetKeyForItem(SvnPropertyValue item)
        {
            return item?.Key;
        }
    }
}
