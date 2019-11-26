using System;
using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container for SvnClient.List</summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnListArgs : SvnClientArgs
    {
        SvnRevision _revision;
        SvnDepth _depth;

        public event EventHandler<SvnListEventArgs> List;

        public virtual void OnList(SvnListEventArgs e)
        {
            List?.Invoke(this, e);
        }

        public SvnListArgs()
        {
            _depth = SvnDepth.Children;
            _revision = SvnRevision.None;
            RetrieveEntries = SvnDirEntryItems.SvnListDefault;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.List;

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        public SvnRevision Revision
        {
            get => _revision;
            set => _revision = value ?? SvnRevision.None;
        }

        public SvnDirEntryItems RetrieveEntries { get; set; }

        public bool RetrieveLocks { get; set; }

        public bool IncludeExternals { get; set; }

        internal string _queryRoot;
        internal Uri _repositoryRoot;

        internal void Prepare(SvnTarget target, bool hasRevision)
        {
            _repositoryRoot = null;

            if (target is SvnUriTarget uriTarget && !hasRevision)
                _queryRoot = SvnBase.UriToCanonicalString(uriTarget.Uri);
            else
                _queryRoot = null;
        }

        internal unsafe Uri CalculateRepositoryRoot(sbyte* absPathPtr)
        {
            if (_repositoryRoot != null || _queryRoot == null)
                return _repositoryRoot;

            var qr = _queryRoot;
            _queryRoot = null; // Only parse in the first call, which matches the exact request

            var absPath = SvnBase.Utf8_PtrToString(absPathPtr + 1);

            var path = absPath.Length > 0
                ? SvnBase.PathToUri(SvnBase.Utf8_PtrToString(absPathPtr + 1)).ToString() // Skip the initial '/'
                : "";

            if (path.Length == 0)
            {
                // Special case. The passed path is the solution root; EndsWith would always succeed
                if (qr.Length > 0 && qr[qr.Length - 1] != '/')
                    _repositoryRoot = new Uri(qr + "/");
                else
                    _repositoryRoot = new Uri(qr);
            }
            else if (Uri.TryCreate(qr, UriKind.Absolute, out var rt))
            {
                var n = path.Length;

                var up = 0;
                while (--n > 0)
                {
                    if (path[n] == '/')
                        up++;
                }

                if (qr[qr.Length - 1] == '/')
                    up++;
                else if (up == 0)
                    rt = new Uri(rt, "./");

                while (up-- > 0)
                    rt = new Uri(rt, "../");

                _repositoryRoot = rt;
            }

            return _repositoryRoot;
        }
    }
}
