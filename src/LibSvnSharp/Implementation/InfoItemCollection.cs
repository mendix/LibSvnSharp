using System;
using System.Collections.ObjectModel;

namespace LibSvnSharp.Implementation
{
    class InfoItemCollection<T> : Collection<T>
        where T : SvnEventArgs
    {
        internal void HandleItem(object sender, T e)
        {
            e.Detach();
            Add(e);
        }

        internal EventHandler<T> Handler => HandleItem;
    }
}
