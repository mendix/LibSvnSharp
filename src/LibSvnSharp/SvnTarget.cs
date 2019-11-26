using System;
using LibSvnSharp.Implementation;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public abstract class SvnTarget : SvnBase, IEquatable<SvnTarget>
    {
        internal SvnTarget(SvnRevision revision)
        {
            Revision = revision ?? SvnRevision.None;
        }

        /// <summary>Gets the operational revision</summary>
        public SvnRevision Revision { get; }

        /// <summary>Gets the target name in normalized format</summary>
        public abstract string TargetName { get; }

        public abstract string FileName { get; }

        internal abstract unsafe sbyte* AllocAsString(AprPool pool, bool absolute);

        internal unsafe sbyte* AllocAsString(AprPool pool)
        {
            return AllocAsString(pool, false);
        }

        /// <summary>Gets the SvnTarget as string</summary>
        public override string ToString()
        {
            if (Revision.RevisionType == SvnRevisionType.None)
                return TargetName;

            return TargetName + "@" + Revision.ToString();
        }

        public static SvnTarget FromUri(Uri value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new SvnUriTarget(value);
        }

        public static SvnTarget FromString(string value)
        {
            return FromString(value, false);
        }

        public static SvnTarget FromString(string value, bool allowOperationalRevision)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (SvnTarget.TryParse(value, allowOperationalRevision, out var result))
                return result;

            throw new ArgumentException(SharpSvnStrings.TheTargetIsNotAValidUriOrPathTarget, nameof(value));
        }

        public static implicit operator SvnTarget(Uri value)
        {
            return value != null ? FromUri(value) : null;
        }

        public static implicit operator SvnTarget(string value)
        {
            return value != null ? FromString(value, false) : null;
        }

        /*
        public static implicit operator SvnTarget(ISvnOrigin origin)
        {
            return origin != null ? origin.Target : null;
        }
        */

        public override bool Equals(object obj)
        {
            return Equals(obj as SvnTarget);
        }

        public virtual bool Equals(SvnTarget other)
        {
            if (other is null)
                return false;

            if (!string.Equals(other.TargetName, TargetName))
                return false;

            return Revision.Equals(other.Revision);
        }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return TargetName.GetHashCode();
        }

        public static bool TryParse(string targetName, out SvnTarget target)
        {
            return TryParse(targetName, false, out target);
        }

        public static bool TryParse(string targetName, bool allowOperationalRevision, out SvnTarget target)
        {
            if (string.IsNullOrEmpty(targetName))
                throw new ArgumentNullException(nameof(targetName));

            if (targetName.Contains("://") && SvnUriTarget.TryParse(targetName, allowOperationalRevision, out var uriTarget))
            {
                target = uriTarget;
                return true;
            }

            if (SvnPathTarget.TryParse(targetName, allowOperationalRevision, out var pathTarget))
            {
                target = pathTarget;
                return true;
            }

            target = null;
            return false;
        }

        internal abstract SvnRevision GetSvnRevision(SvnRevision fileNoneValue, SvnRevision uriNoneValue);
    }
}
