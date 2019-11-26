using System.Collections.ObjectModel;

namespace LibSvnSharp
{
    public sealed class SvnRevisionPropertyCollection : KeyedCollection<string, SvnPropertyValue>
    {
        protected override string GetKeyForItem(SvnPropertyValue item)
        {
            return item?.Key;
        }

        public void Add(string key, string value)
        {
            Add(new SvnPropertyValue(key, value));
        }

        public void Add(string key, byte[] value)
        {
            Add(new SvnPropertyValue(key, value));
        }
    }
}
