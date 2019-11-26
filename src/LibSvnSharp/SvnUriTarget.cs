using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public sealed class SvnUriTarget : SvnTarget
    {
        public SvnUriTarget(Uri uri, SvnRevision revision)
            : base(revision)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(SharpSvnStrings.UriIsNotAbsolute, nameof(uri));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uri));

            Uri = CanonicalizeUri(uri);
        }

        public SvnUriTarget(Uri uri)
            : base(SvnRevision.None)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(SharpSvnStrings.UriIsNotAbsolute, nameof(uri));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uri));

            Uri = CanonicalizeUri(uri);
        }

        public SvnUriTarget(string uriString, SvnRevision revision)
            : base(revision)
        {
            if (string.IsNullOrEmpty(uriString))
                throw new ArgumentNullException(nameof(uriString));

            var uri = new Uri(uriString);

            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(SharpSvnStrings.UriIsNotAbsolute, nameof(uriString));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uriString));

            Uri = CanonicalizeUri(uri);
        }

        public SvnUriTarget(string uriString)
            : base(SvnRevision.None)
        {
            if (string.IsNullOrEmpty(uriString))
                throw new ArgumentNullException(nameof(uriString));

            var uri = new Uri(uriString);

            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(SharpSvnStrings.UriIsNotAbsolute, nameof(uriString));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uriString));

            Uri = CanonicalizeUri(uri);
        }

        public SvnUriTarget(Uri uri, long revision)
            : base(new SvnRevision(revision))
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(SharpSvnStrings.UriIsNotAbsolute, nameof(uri));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uri));

            Uri = CanonicalizeUri(uri);
        }

        public SvnUriTarget(Uri uri, DateTime date)
            : base(new SvnRevision(date))
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(SharpSvnStrings.UriIsNotAbsolute, nameof(uri));
            if (!IsValidReposUri(uri))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAValidRepositoryUri, nameof(uri));

            Uri = CanonicalizeUri(uri);
        }

        public new static SvnUriTarget FromUri(Uri value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new SvnUriTarget(value);
        }

        public new static SvnUriTarget FromString(string value)
        {
            return FromString(value, false);
        }

        public new static SvnUriTarget FromString(string value, bool allowOperationalRevision)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (SvnUriTarget.TryParse(value, allowOperationalRevision, out var result))
                return result;

            throw new ArgumentException(SharpSvnStrings.TheTargetIsNotAValidUriTarget, nameof(value));
        }

        public static implicit operator SvnUriTarget(Uri value)
        {
            return value != null ? FromUri(value) : null;
        }

        public static explicit operator SvnUriTarget(string value)
        {
            return value != null ? FromString(value) : null;
        }

        /*
        public static implicit operator SvnUriTarget(ISvnOrigin origin)
        {
            return origin != null ? new SvnUriTarget(origin.Uri, origin.Target.Revision) : null;
        }
        */

        public Uri Uri { get; }

        public override string TargetName => UriToString(Uri);

        public override string FileName => SvnTools.GetFileName(Uri);

        internal override unsafe sbyte* AllocAsString(AprPool pool, bool absolute)
        {
            return pool.AllocUri(Uri);
        }

        public static bool TryParse(string path, out SvnUriTarget pegUri)
        {
            return TryParse(path, false, out pegUri);
        }

        public static bool TryParse(string path, bool allowOperationalRevision, out SvnUriTarget pegUri)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            using (var pool = new AprPool())
            {
                return TryParse(path, allowOperationalRevision, out pegUri, pool);
            }
        }

        public static ICollection<SvnUriTarget> Map(IEnumerable<Uri> uris)
        {
            if (uris is null)
                throw new ArgumentNullException(nameof(uris));

            var targets = new List<SvnUriTarget>();

            foreach (Uri uri in uris)
            {
                targets.Add(uri);
            }

            return targets;
        }

        internal void VerifyBelowRoot(Uri repositoryRoot)
        {
            // TODO: Throw exception if the current value is not below the repository root
        }

        internal static unsafe bool TryParse(string targetString, bool allowOperationalRevision, out SvnUriTarget target, AprPool pool)
        {
            if (string.IsNullOrEmpty(targetString))
                throw new ArgumentNullException(nameof(targetString));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (allowOperationalRevision)
            {
                svn_error_t r;
                sbyte* truePath;

                var path = pool.AllocString(targetString);

                if ((r = svn_opt.svn_opt_parse_path(out svn_opt_revision_t rev, &truePath, path, pool.Handle)) == null)
                {
                    if (Uri.TryCreate(Utf8_PtrToString(truePath), UriKind.Absolute, out var uri))
                    {
                        var pegRev = SvnRevision.Load(rev);

                        target = new SvnUriTarget(uri, pegRev);
                        return true;
                    }
                }
                else
                    svn_error.svn_error_clear(r);
            }
            else
            {
                if (Uri.TryCreate(targetString, UriKind.Absolute, out var uri))
                {
                    target = new SvnUriTarget(uri);
                    return true;
                }
            }

            target = null;
            return false;
        }

        internal override SvnRevision GetSvnRevision(SvnRevision fileNoneValue, SvnRevision uriNoneValue)
        {
            return Revision.Or(uriNoneValue);
        }
    }
}
