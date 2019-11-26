using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Apr;

namespace LibSvnSharp
{
    public sealed class SvnCommittingEventArgs : SvnEventArgs
    {
        AprPool _pool;
        apr_array_header_t _commitItems;
        SvnCommitItemCollection _items;

        internal SvnCommittingEventArgs(apr_array_header_t commitItems, SvnCommandType commandType, AprPool pool)
        {
            if (commitItems == null)
                throw new ArgumentNullException(nameof(commitItems));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _commitItems = commitItems;
            _pool = pool;
            CurrentCommandType = commandType;
        }

        public bool Cancel { get; set; }

        public string LogMessage { get; set; }

        public SvnCommitItemCollection Items
        {
            get
            {
                if (_items == null && _commitItems != null)
                {
                    var aprItems = new AprArray<SvnCommitItem, SvnCommitItemMarshaller>(_commitItems, _pool);

                    var items = new SvnCommitItem[aprItems.Count];

                    aprItems.CopyTo(items, 0);

                    _items = new SvnCommitItemCollection(items);
                }

                return _items;
            }
        }

        public SvnCommandType CurrentCommandType { get; }

        protected internal override void Detach(bool keepProperties)
        {
            try
            {
                if (keepProperties)
                {
                    if (Items != null)
                    {
                        foreach (SvnCommitItem item in Items)
                        {
                            item.Detach(true);
                        }
                    }
                }
            }
            finally
            {
                _commitItems = null;
                _pool = null;
            }
        }
    }
}
