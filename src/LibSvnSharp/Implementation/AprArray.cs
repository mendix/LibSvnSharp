using System;
using System.Collections;
using System.Collections.Generic;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Properties;

namespace LibSvnSharp.Implementation
{
    class AprArray<TManaged, TMarshaller> : SvnBase, IDisposable
        where TMarshaller : IItemMarshaller<TManaged>
    {
        AprPool _pool;
        apr_array_header_t _handle;
        readonly IItemMarshaller<TManaged> _marshaller;

        internal AprArray(ICollection<TManaged> items, AprPool pool)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _marshaller = Activator.CreateInstance<TMarshaller>();
            _pool = pool;
            _handle = apr_tables.apr_array_make(pool.Handle, items.Count, _marshaller.ItemSize);

            foreach (var t in items)
            {
                var ptr = apr_tables.apr_array_push(_handle);

                _marshaller.Write(t, ptr, pool);
            }
        }

        internal AprArray(apr_array_header_t handle, AprPool pool)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _marshaller = Activator.CreateInstance<TMarshaller>();
            _handle = handle;
            _pool = pool;
            IsReadOnly = true;
        }

        internal AprArray(IEnumerable items, AprPool pool)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            int nItems = 0;
            foreach (TManaged t in items)
            {
                if (ReferenceEquals(t, null))
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(items));

                nItems++;
            }

            _marshaller = Activator.CreateInstance<TMarshaller>();
            _pool = pool;
            _handle = apr_tables.apr_array_make(pool.Handle, nItems, _marshaller.ItemSize);

            foreach (TManaged t in items)
            {
                var ptr = apr_tables.apr_array_push(_handle);

                _marshaller.Write(t, ptr, pool);
            }
        }

        internal apr_array_header_t Handle
        {
            get
            {
                _pool.Ensure();
                return _handle;
            }
        }

        public int Count => _handle.nelts;

        public unsafe TManaged this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");

                sbyte* pData = Handle.elts;

                return _marshaller.Read(new IntPtr(pData + _marshaller.ItemSize * index), _pool);
            }
        }

        public TManaged[] ToArray()
        {
            var items = new TManaged[Count];

            CopyTo(items, 0);

            return items;
        }

        public void CopyTo(TManaged[] item, int offset)
        {
            for (int i = 0; i < Count; i++)
                item[i + offset] = this[i];
        }

        public bool IsReadOnly { get; }

        public void Dispose()
        {
            _handle = null;
        }
    }
}
