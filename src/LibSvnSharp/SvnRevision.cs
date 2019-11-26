using System;
using System.Globalization;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public sealed class SvnRevision : IEquatable<SvnRevision>
    {
        readonly long _value;

        public SvnRevision()
        {
            RevisionType = SvnRevisionType.None;
        }

        public SvnRevision(int revision)
        {
            if (revision < 0)
                throw new ArgumentOutOfRangeException(nameof(revision), revision, SharpSvnStrings.RevisionMustBeGreaterThanOrEqualToZero);

            RevisionType = SvnRevisionType.Number;
            _value = revision;
        }

        public SvnRevision(long revision)
        {
            if (revision < 0)
                throw new ArgumentOutOfRangeException(nameof(revision), revision, SharpSvnStrings.RevisionMustBeGreaterThanOrEqualToZero);

            RevisionType = SvnRevisionType.Number;
            _value = revision;
        }

        public SvnRevision(SvnRevisionType type)
        {
            switch (type)
            {
                case SvnRevisionType.None:
                case SvnRevisionType.Committed:
                case SvnRevisionType.Previous:
                case SvnRevisionType.Base:
                case SvnRevisionType.Working:
                case SvnRevisionType.Head:
                    RevisionType = type;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public SvnRevision(DateTime date)
        {
            RevisionType = SvnRevisionType.Time;
            _value = date.ToUniversalTime().Ticks;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SvnRevision);
        }

        public bool Equals(SvnRevision other)
        {
            if (other is null)
                return false;

            if (other.RevisionType != RevisionType)
                return false;

            switch (RevisionType)
            {
                case SvnRevisionType.None:
                case SvnRevisionType.Committed:
                case SvnRevisionType.Previous:
                case SvnRevisionType.Base:
                case SvnRevisionType.Working:
                case SvnRevisionType.Head:
                    return true;
                case SvnRevisionType.Number:
                case SvnRevisionType.Time:
                    return _value == other._value;
                default:
                    return false;
            }
        }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return RevisionType.GetHashCode() ^ _value.GetHashCode();
        }

        internal static SvnRevision Load(svn_opt_revision_t revData)
        {
            if (revData == null)
                throw new ArgumentNullException(nameof(revData));

            var type = (SvnRevisionType) revData.kind;

            switch (type)
            {
                case SvnRevisionType.None:
                    return None;
                case SvnRevisionType.Committed:
                    return Committed;
                case SvnRevisionType.Previous:
                    return Previous;
                case SvnRevisionType.Base:
                    return Base;
                case SvnRevisionType.Working:
                    return Working;
                case SvnRevisionType.Head:
                    return Head;
                case SvnRevisionType.Number:
                    if (revData.value.number == 0)
                        return Zero;
                    if (revData.value.number == 1)
                        return One;
                    return new SvnRevision(revData.value.number);
                case SvnRevisionType.Time:
                    // apr_time_t is in microseconds since 1-1-1970 UTC; filetime is in 100 nanoseconds
                    return new SvnRevision(SvnBase.DateTimeFromAprTime(revData.value.date));
                default:
                    throw new ArgumentException(SharpSvnStrings.InvalidSvnRevisionTypeValue, nameof(revData));
            }
        }

        public override string ToString()
        {
            switch (RevisionType)
            {
                case SvnRevisionType.None:
                    return "";
                case SvnRevisionType.Number:
                    return _value.ToString(CultureInfo.InvariantCulture);
                case SvnRevisionType.Time:
                    return "{" + new DateTime(_value, DateTimeKind.Utc).ToString("s", CultureInfo.InvariantCulture) + "}";
                case SvnRevisionType.Committed:
                    return "COMMITTED";
                case SvnRevisionType.Previous:
                    return "PREVIOUS";
                case SvnRevisionType.Base:
                    return "BASE";
                case SvnRevisionType.Working:
                    return "WORKING";
                case SvnRevisionType.Head:
                    return "HEAD";
                default:
                    return null;
            }
        }

        public SvnRevisionType RevisionType { get; }

        public long Revision => RevisionType == SvnRevisionType.Number ? _value : -1;

        public DateTime Time
        {
            get
            {
                if (RevisionType == SvnRevisionType.Time)
                    return new DateTime(_value, DateTimeKind.Utc);

                return DateTime.MinValue;
            }
        }

        /// <summary>Gets a boolean indicating whether the revisionnumber requires a workingcopy to make any sense</summary>
        public bool RequiresWorkingCopy
        {
            get
            {
                switch (RevisionType)
                {
                    case SvnRevisionType.None:
                    case SvnRevisionType.Time:
                    case SvnRevisionType.Head:
                    case SvnRevisionType.Number:
                        return false;
                    default:
                        return true;
                }
            }
        }

        /// <summary>Gets a boolean whether the revision specifies an explicit repository revision</summary>
        /// <remarks>Explicit: Is set, doesn't require a workingcopy and is repository wide applyable</remarks>
        public bool IsExplicit => (RevisionType != SvnRevisionType.None) && !RequiresWorkingCopy;

        public static bool operator ==(SvnRevision rev1, SvnRevision rev2)
        {
            if (rev1 is null)
                return rev2 is null;
            if (rev2 is null)
                return false;

            return rev1.Equals(rev2);
        }

        public static bool operator !=(SvnRevision rev1, SvnRevision rev2)
        {
            return !(rev1 == rev2);
        }

        public static implicit operator SvnRevision(long value)
        {
            if (value == 0)
                return Zero;
            if (value == 1)
                return One;

            return new SvnRevision(value);
        }

        public static implicit operator SvnRevision(DateTime value)
        {
            return new SvnRevision(value);
        }

        public static implicit operator SvnRevision(SvnRevisionType value)
        {
            switch (value)
            {
                case SvnRevisionType.None:
                    return None;
                case SvnRevisionType.Head:
                    return Head;
                case SvnRevisionType.Working:
                    return Working;
                case SvnRevisionType.Base:
                    return Base;
                case SvnRevisionType.Previous:
                    return Previous;
                case SvnRevisionType.Committed:
                    return Committed;
                default:
                    return new SvnRevision(value);
            }
        }

        internal SvnRevision Or(SvnRevision alternate)
        {
            if (RevisionType != SvnRevisionType.None)
                return this;

            return alternate ?? SvnRevision.None;
        }

        internal svn_opt_revision_t.__Internal ToSvnRevision()
        {
            var r = new svn_opt_revision_t.__Internal();
            r.kind = (svn_opt_revision_kind) RevisionType; // Values are identical by design

            switch (RevisionType)
            {
                case SvnRevisionType.Number:
                    r.value.number = (int) _value;
                    break;
                case SvnRevisionType.Time:
                    r.value.date = SvnBase.AprTimeFromDateTime(new DateTime(_value, DateTimeKind.Utc));
                    break;
            }

            return r;
        }

        internal unsafe svn_opt_revision_t AllocSvnRevision(AprPool pool)
        {
            if (pool is null)
                throw new ArgumentNullException(nameof(pool));

            var rev = (svn_opt_revision_t.__Internal*) pool.Alloc(
                sizeof(svn_opt_revision_t.__Internal));

            *rev = ToSvnRevision();

            return svn_opt_revision_t.__CreateInstance(new IntPtr(rev));
        }

        public static SvnRevision None { get; } = new SvnRevision(SvnRevisionType.None);
        public static SvnRevision Head { get; } = new SvnRevision(SvnRevisionType.Head);
        public static SvnRevision Working { get; } = new SvnRevision(SvnRevisionType.Working);
        public static SvnRevision Base { get; } = new SvnRevision(SvnRevisionType.Base);
        public static SvnRevision Previous { get; } = new SvnRevision(SvnRevisionType.Previous);
        public static SvnRevision Committed { get; } = new SvnRevision(SvnRevisionType.Committed);
        public static SvnRevision Zero { get; } = new SvnRevision(0L);
        public static SvnRevision One { get; } = new SvnRevision(1L);
    }
}
