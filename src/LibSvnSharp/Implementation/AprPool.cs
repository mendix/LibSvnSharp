using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Implementation
{
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    sealed class AprPool : SvnHandleBase, IDisposable
    {
        sealed class AprPoolTag : IDisposable
        {
            bool _disposed;
            AprPoolTag _parent;

            public AprPoolTag()
            {
            }

            public AprPoolTag(AprPoolTag parent)
            {
                _parent = parent;
            }

            public void Ensure()
            {
                if (_disposed)
                    throw new ObjectDisposedException("AprPool");

                _parent?.Ensure();
            }

            internal bool IsValid()
            {
                if (_disposed)
                    return false;

                if (_parent != null)
                    return _parent.IsValid();

                return true;
            }

            public void Dispose()
            {
                _disposed = true;
                _parent = null;
            }
        }

        AprPool _parent;
        AprPoolTag _tag;
        apr_pool_t _handle;
        readonly bool _destroyPool;

        /// <summary>Creates a childpool within the specified parent pool</summary>
        public AprPool(AprPool parentPool)
        {
            if (parentPool == null)
                throw new ArgumentNullException(nameof(parentPool));

            _tag = new AprPoolTag(parentPool._tag);
            _parent = parentPool;
            _handle = svn_pools.svn_pool_create(parentPool.Handle);
            _destroyPool = true;
        }

        /// <summary>Creates a new root pool</summary>
        public AprPool()
        {
            _tag = new AprPoolTag();
            _handle = svn_pools.svn_pool_create(_ultimateParentPool);
            _destroyPool = true;
        }

        /// <summary>Attaches to the specified pool</summary>
        public AprPool(apr_pool_t handle, bool destroyPool)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
            _tag = new AprPoolTag();
            _destroyPool = destroyPool;
        }

        /// <summary>Attaches to the specified pool</summary>
        public AprPool(IntPtr handle, bool destroyPool)
            : this(apr_pool_t.__CreateInstance(handle), destroyPool)
        {
        }

        public void Ensure()
        {
            if (_tag == null)
                throw new ObjectDisposedException("AprPool");

            _tag.Ensure();
        }

        public bool IsValid()
        {
            return _tag != null && _tag.IsValid();
        }

        internal apr_pool_t Handle
        {
            get
            {
                _tag.Ensure();
                return _handle;
            }
        }

        internal void Clear()
        {
            _tag.Ensure();

            _tag.Dispose();

            _tag = _parent != null ? new AprPoolTag(_parent._tag) : new AprPoolTag();

            svn_pools.svn_pool_clear(_handle);
        }

        internal IntPtr Alloc(long size)
        {
            var p = apr_pools.apr_palloc(Handle, unchecked((ulong) size));

            if (p == IntPtr.Zero)
                throw new ArgumentException("apr_palloc returned null; We have crashed before you see this (See svn sourcecode)", nameof(size));

            return p;
        }

        internal unsafe IntPtr AllocCleared(long size)
        {
            var p = Alloc(size);

            // #define apr_pcalloc(p, size) memset(apr_palloc(p, size), 0, size)
            for (long i = 0; i < size; i++)
            {
                var addr = (byte*) p + i;
                *addr = 0;
            }

            return p;
        }

        unsafe sbyte* AllocEmptyString()
        {
            // This differs from SharpSvn because we can't return managed empty string as an sbyte pointer

            var pData = Alloc(1);
            Marshal.WriteByte(pData, 0);

            return (sbyte*) pData.ToPointer();
        }

        internal unsafe sbyte* AllocString(string value)
        {
            if (value == null)
                value = "";

            if (value.Length >= 1)
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                var pData = Alloc(bytes.Length + 1);

                if (pData != IntPtr.Zero)
                    Marshal.Copy(bytes, 0, pData, bytes.Length);

                Marshal.WriteByte(pData, bytes.Length, 0);

                return (sbyte*) pData.ToPointer();
            }

            return AllocEmptyString();
        }

        internal unsafe sbyte* AllocUnixString(string value)
        {
            if (value == null)
                value = "";

            if (value.Length >= 1)
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                var pData = Alloc(bytes.Length + 1);

                if (pData != IntPtr.Zero)
                    Marshal.Copy(bytes, 0, pData, bytes.Length);

                Marshal.WriteByte(pData, bytes.Length, 0);

                sbyte* pFrom;
                sbyte* pTo;

                pFrom = pTo = (sbyte*) pData.ToPointer();

                while (*pFrom != 0)
                {
                    switch (*pFrom)
                    {
                        case (sbyte)'\r':
                            *pTo++ = (sbyte)'\n';
                            if (*(++pFrom) == '\n')
                                pFrom++;
                            break;

                        case (sbyte)'\n':
                            *pTo++ = (sbyte)'\n';
                            if (*(++pFrom) == '\r')
                                pFrom++;
                            break;

                        default:
                            *pTo++ = *pFrom++;
                            break;
                    }
                }

                *pTo = 0;

                return (sbyte*) pData.ToPointer();
            }

            return AllocEmptyString();
        }

        internal unsafe sbyte* AllocDirent(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 1 && value[0] == '.')
                return AllocEmptyString();

            if (value.Length >= 1)
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                int len = bytes.Length;

                while (len != 0 && ((bytes[len - 1] == '\\') || bytes[len - 1] == '/'))
                    len--;

                if (len == 2 && bytes[1] == ':' && value.Length > 2)
                    len++;

                var data = Alloc(len + 1);

                if (data == IntPtr.Zero)
                    throw new InvalidOperationException();

                Marshal.Copy(bytes, 0, data, len);

                var pData = (sbyte*) data.ToPointer();

                // Should match: svn_path_internal_style() implementation, but doesn't copy an extra time
                for (int i = 0; i < len; i++)
                    if (pData[i] == '\\')
                        pData[i] = (sbyte)'/';

                pData[len] = 0;

                if ((len != 0 && pData[len - 1] == '/') || new string(pData, 1, len - 1).Contains("//"))
                    return svn_dirent_uri.svn_dirent_canonicalize(pData, Handle);

                var res = new string(pData, 0, len);

                if (res.Contains("/./") || res.EndsWith("/."))
                    return svn_dirent_uri.svn_dirent_canonicalize(pData, Handle);

                if (pData[0] >= 'a' && pData[0] <= 'z' && pData[1] == ':')
                    pData[0] -= ('a' - 'A');

                return pData;
            }

            return AllocEmptyString();
        }

        internal unsafe sbyte* AllocAbsoluteDirent(string value)
        {
            return AllocDirent(SvnTools.GetNormalizedFullPath(value));
        }

        internal unsafe sbyte* AllocUri(Uri value)
        {
            if (value == null)
                return AllocUri((string) null);

            return AllocUri(SvnBase.UriToString(value));
        }

        internal unsafe sbyte* AllocUri(string value)
        {
            if (value == null)
                value = "";

            if (value.Length >= 1)
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                for (int i = 0; i < bytes.Length; i++)
                    if (bytes[i] == '\\')
                        bytes[i] = (byte)'/';

                fixed (byte* pBytes = &bytes[0])
                {
                    sbyte* pcBytes = (sbyte*) pBytes;

                    sbyte* resPath = svn_dirent_uri.svn_uri_canonicalize(pcBytes, Handle);

                    if (resPath == pcBytes)
                        resPath = apr_strings.apr_pstrdup(Handle, resPath);

                    return resPath;
                }
            }

            return AllocEmptyString();
        }

        internal unsafe svn_string_t AllocSvnString(string value)
        {
            if (value == null)
                value = "";

            var pStr = (svn_string_t.__Internal*) AllocCleared(
                sizeof(svn_string_t.__Internal));

            pStr->data = new IntPtr(AllocString(value));
            pStr->len = (ulong) SvnBase.strlen((byte*) pStr->data);

            return svn_string_t.__CreateInstance(new IntPtr(pStr));
        }

        internal unsafe svn_string_t AllocPropertyValue(string value, string propertyName)
        {
            sbyte* propName = AllocString(propertyName);

            if (svn_props.svn_prop_is_boolean(propName))
                return AllocSvnString(value != null ? Constants.SVN_PROP_BOOLEAN_TRUE : "");

            if (svn_props.svn_prop_needs_translation(propName))
                return AllocUnixSvnString(value);

            return AllocSvnString(value);
        }

        internal unsafe svn_string_t AllocUnixSvnString(string value)
        {
            if (value == null)
                value = "";

            var pStr = (svn_string_t.__Internal*) AllocCleared(
                sizeof(svn_string_t.__Internal));

            pStr->data = new IntPtr(AllocUnixString(value));
            pStr->len = (ulong) SvnBase.strlen((byte*) pStr->data);

            return svn_string_t.__CreateInstance(new IntPtr(pStr));
        }

        internal unsafe svn_string_t AllocSvnString(byte[] bytes)
        {
            if (bytes == null)
                bytes = Array.Empty<byte>();

            var pStr = (svn_string_t.__Internal*) AllocCleared(
                sizeof(svn_string_t.__Internal));

            var pChars = AllocCleared(bytes.Length + 1);
            pStr->data = pChars;
            pStr->len = (ulong) bytes.Length;

            if (bytes.Length > 0)
                Marshal.Copy(bytes, 0, pChars, bytes.Length);

            return svn_string_t.__CreateInstance(new IntPtr(pStr));
        }

        void Destroy()
        {
            if (_handle == null)
                return;

            var handle = _handle;
            _handle = null;

            var valid = (_tag != null) && _tag.IsValid(); // Don't crash the finalizer; dont Destroy if parent is deleted

            if (valid)
            {
                _tag.Dispose();
                _tag = null;

                if (_destroyPool)
                {
                    try
                    {
                        svn_pools.svn_pool_destroy(handle);
                    }
                    catch { }
                }
            }
        }

        public void Dispose()
        {
            Destroy();
            GC.SuppressFinalize(this);
        }

        ~AprPool()
        {
            Destroy();
        }
    }
}
