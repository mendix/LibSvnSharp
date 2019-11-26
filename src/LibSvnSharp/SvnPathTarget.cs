using System;
using System.Collections.Generic;
using System.IO;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public sealed class SvnPathTarget : SvnTarget
    {
        static string GetFullTarget(string path)
        {
            return SvnTools.GetNormalizedFullPath(path);
        }

        public static string GetTargetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            if (SvnTools.IsAbsolutePath(path))
                return SvnTools.GetNormalizedFullPath(path);

            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var dualSeparator = string.Concat(Path.DirectorySeparatorChar, Path.DirectorySeparatorChar);

            int nNext;
            // Remove double backslash except at the start
            while ((nNext = path.IndexOf(dualSeparator, 1, StringComparison.Ordinal)) >= 0)
                path = path.Remove(nNext, 1);

            // Remove '\.\'
            while ((nNext = path.IndexOf("\\.\\", StringComparison.Ordinal)) >= 0)
                path = path.Remove(nNext, 2);

            while (path.StartsWith(".\\", StringComparison.Ordinal))
                path = path.Substring(2);

            if (path.EndsWith("\\.", StringComparison.Ordinal))
                path = path.Substring(0, path.Length - 2);

            path = path.TrimEnd(Path.DirectorySeparatorChar);

            if (path.Length == 0)
                path = ".";

            if (path.Length > 2 && path[1] == ':' && path[0] >= 'a' && path[0] <= 'z')
                path = char.ToUpperInvariant(path[0]) + path.Substring(1);

            return path;
        }

        public SvnPathTarget(string path, SvnRevision revision)
            : base(revision)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            TargetPath = GetTargetPath(path);
            FullPath = GetFullTarget(TargetPath);
        }

        public SvnPathTarget(string path)
            : base(SvnRevision.None)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            TargetPath = GetTargetPath(path);
            FullPath = GetFullTarget(TargetPath);
        }

        public SvnPathTarget(string path, long revision)
            : base(new SvnRevision(revision))
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            TargetPath = GetTargetPath(path);
            FullPath = GetFullTarget(TargetPath);
        }

        public SvnPathTarget(string path, DateTime date)
            : base(new SvnRevision(date))
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            TargetPath = GetTargetPath(path);
            FullPath = GetFullTarget(TargetPath);
        }

        public SvnPathTarget(string path, SvnRevisionType type)
            : base(new SvnRevision(type))
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!IsNotUri(path))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(path));

            TargetPath = GetTargetPath(path);
            FullPath = GetFullTarget(TargetPath);
        }

        public override string TargetName => TargetPath;

        public override string FileName => Path.GetFileName(TargetPath);

        internal override unsafe sbyte* AllocAsString(AprPool pool, bool absolute)
        {
            return absolute
                ? pool.AllocAbsoluteDirent(TargetPath)
                : pool.AllocDirent(TargetPath);
        }

        public string TargetPath { get; }

        public string FullPath { get; }

        public static bool TryParse(string targetName, out SvnPathTarget target)
        {
            return TryParse(targetName, false, out target);
        }

        public static bool TryParse(string targetName, bool allowPegRevision, out SvnPathTarget target)
        {
            if (string.IsNullOrEmpty(targetName))
                throw new ArgumentNullException(nameof(targetName));

            target = null;

            if (!IsNotUri(targetName))
                return false;

            if (allowPegRevision)
            {
                using (var pool = new AprPool(SmallThreadPool))
                    return TryParse(targetName, allowPegRevision, out target, pool);
            }

            target = new SvnPathTarget(targetName);
            return true;
        }

        public static ICollection<SvnPathTarget> Map(IEnumerable<string> paths)
        {
            if (paths is null)
                throw new ArgumentNullException(nameof(paths));

            var targets = new List<SvnPathTarget>();

            foreach (string path in paths)
            {
                targets.Add(path);
            }

            return targets;
        }

        internal static unsafe bool TryParse(string targetName, bool allowOperationalRevisions, out SvnPathTarget target, AprPool pool)
        {
            if (string.IsNullOrEmpty(targetName))
                throw new ArgumentNullException(nameof(targetName));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            target = null;

            if (!IsNotUri(targetName))
                return false;

            if (allowOperationalRevisions)
            {
                svn_error_t r;
                sbyte* truePath;

                var path = pool.AllocDirent(targetName);

                if ((r = svn_opt.svn_opt_parse_path(out svn_opt_revision_t rev, &truePath, path, pool.Handle)) == null)
                {
                    var realPath = Utf8_PtrToString(truePath);

                    if (!realPath.Contains("://"))
                    {
                        var pegRev = SvnRevision.Load(rev);

                        target = new SvnPathTarget(realPath, pegRev);
                        return true;
                    }
                }
                else
                    svn_error.svn_error_clear(r);
            }
            else
            {
                target = new SvnPathTarget(targetName);
                return true;
            }

            return false;
        }

        public new static SvnPathTarget FromString(string value)
        {
            return FromString(value, false);
        }

        public new static SvnPathTarget FromString(string value, bool allowOperationalRevision)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (SvnPathTarget.TryParse(value, allowOperationalRevision, out var result))
                return result;

            throw new ArgumentException(SharpSvnStrings.TheTargetIsNotAValidPathTarget, nameof(value));
        }

        public static implicit operator SvnPathTarget(string value)
        {
            return value != null ? new SvnPathTarget(value) : null;
        }

        internal override SvnRevision GetSvnRevision(SvnRevision fileNoneValue, SvnRevision uriNoneValue)
        {
            return Revision.Or(fileNoneValue);
        }
    }
}
