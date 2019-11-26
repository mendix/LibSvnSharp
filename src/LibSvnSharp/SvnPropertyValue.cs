using System;
using System.Collections.Generic;
using System.Text;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [System.Diagnostics.DebuggerDisplay("{Key}={StringValue}")]
    public sealed class SvnPropertyValue : IEquatable<SvnPropertyValue>
    {
        byte[] _value;

        public SvnPropertyValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Key = key;
            StringValue = value;
        }

        public SvnPropertyValue(string key, byte[] value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Key = key;
            _value = value;
        }

        public SvnPropertyValue(string key, SvnPropertyValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Key = key;
            StringValue = value.StringValue;
            _value = value._value;
        }

        /*internal SvnPropertyValue(String key, String value, SvnTarget target)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            else if (!value)
                throw new ArgumentNullException("value");

            _target = target;
            _key = key;
            _strValue = value;
        }*/

        internal SvnPropertyValue(string key, byte[] value, SvnTarget target)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Target = target;
            Key = key;
            _value = value;
        }

        internal SvnPropertyValue(string key, byte[] value, string strValue, SvnTarget target)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(value));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Target = target;
            Key = key;
            _value = value;
            StringValue = strValue;
        }

        internal static unsafe SvnPropertyValue Create(sbyte* propertyName, svn_string_t value, SvnTarget target)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            string name = SvnBase.Utf8_PtrToString(propertyName);

            return Create(propertyName, value, target, name);
        }

        internal static unsafe SvnPropertyValue Create(sbyte* propertyName, svn_string_t value, SvnTarget target, string name)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            object val = SvnBase.PtrToStringOrByteArray(value.data, (int)value.len);

            if (val is string strVal)
            {
                if (svn_props.svn_prop_needs_translation(propertyName))
                    strVal = strVal.Replace("\n", Environment.NewLine);

                return new SvnPropertyValue(name, SvnBase.PtrToByteArray(value.data, (int)value.len), strVal, target);
            }

            return new SvnPropertyValue(name, (byte[])val, target);
        }

        /// <summary>Gets the <see cref="SvnTarget" /> the <see cref="SvnPropertyValue" /> applies to;
        /// <c>null</c> if it applies to a revision property</summary>
        public SvnTarget Target { get; }

        public string Key { get; }

        public string StringValue { get; }

        public ICollection<byte> RawValue => _value ?? (_value = Encoding.UTF8.GetBytes(StringValue));

        public byte[] ToByteArray()
        {
            ICollection<byte> v = RawValue;

            byte[] list = new byte[v.Count];
            v.CopyTo(list, 0);

            return list;
        }

        public override string ToString()
        {
            return StringValue ?? "<raw>";
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            SvnPropertyValue ob = obj as SvnPropertyValue;

            return Equals(ob);
        }

        public bool Equals(SvnPropertyValue other)
        {
            if (other == null)
                return false;

            if ((Target is null) != (other.Target is null))
                return false;
            if (Target != null && !Target.Equals(other.Target))
                return false;

            if (!string.Equals(Key, other.Key, StringComparison.Ordinal))
                return false;

            return ValueEquals(other);
        }

        public bool ValueEquals(SvnPropertyValue other)
        {
            if (other == null)
                return false;

            if (StringValue != null && other.StringValue != null)
                return string.Equals(StringValue, other.StringValue);

            if (RawValue.Count != other.RawValue.Count)
                return false;

            IEnumerator<byte> vMe = RawValue.GetEnumerator();
            IEnumerator<byte> vHe = other.RawValue.GetEnumerator();

            while(vMe.MoveNext() && vHe.MoveNext())
            {
                if(vMe.Current != vHe.Current)
                    return false;
            }

            return true;
        }
    }
}
