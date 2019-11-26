using System;
using System.Collections.Generic;
using System.Globalization;
using LibSvnSharp.Properties;

namespace LibSvnSharp.Implementation
{
    sealed class EnumVerifier
    {
        EnumVerifier()
        {
        }

        sealed class EnumVerifyHelper<T> : IComparer<T>
            where T : Enum
        {
            static readonly T[] _values;
            static readonly EnumVerifyHelper<T> _default;

            static EnumVerifyHelper()
            {
                _values = (T[]) (Enum.GetValues(typeof(T)));
                _default = new EnumVerifyHelper<T>();
                Array.Sort(_values, _default);
            }

            public static bool IsDefined(T value)
            {
                return 0 <= Array.BinarySearch(_values,
                           value,
                           _default);
            }

            public int Compare(T x, T y)
            {
                return (int) (object) x - (int) (object) y;
            }
        }

        public static T Verify<T>(T value)
            where T : Enum
        {
            if (!EnumVerifyHelper<T>.IsDefined(value))
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(CultureInfo.InvariantCulture, SharpSvnStrings.VerifyEnumFailed, value, typeof(T).FullName));

            return value;
        }

        public static bool IsValueDefined<T>(T value)
            where T : Enum
        {
            return EnumVerifyHelper<T>.IsDefined(value);
        }
    }
}
