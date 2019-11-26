using System.Collections.ObjectModel;

namespace LibSvnSharp
{
    public sealed class SvnTargetPropertyCollection : KeyedCollection<SvnTarget, SvnPropertyValue>
    {
        protected override SvnTarget GetKeyForItem(SvnPropertyValue item)
        {
            return item?.Target;
        }
    }
}
