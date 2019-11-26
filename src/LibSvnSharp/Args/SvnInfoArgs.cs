using System;
using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of SvnClient.Info</summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnInfoArgs : SvnClientArgs
    {
        SvnRevision _revision;
        SvnDepth _depth;
        SvnChangeListCollection _changelists;
        bool _filterExcluded;
        bool _filterActualOnly;

        public event EventHandler<SvnInfoEventArgs> Info;

        protected internal void OnInfo(SvnInfoEventArgs e)
        {
            Info?.Invoke(this, e);
        }

        public SvnInfoArgs()
        {
            _revision = SvnRevision.None;
            _depth = SvnDepth.Empty;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.Info;

        /// <summary>Include excluded nodes in the result (Default true)</summary>
        public bool RetrieveExcluded
        {
            get => !_filterExcluded;
            set => _filterExcluded = !value;
        }

        /// <summary>Include actual only (tree conflict) nodes in the result (Default true)</summary>
        public bool RetrieveActualOnly
        {
            get => !_filterActualOnly;
            set => _filterActualOnly = !value;
        }

        public bool IncludeExternals { get; set; }

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

        /// <summary>Gets the list of changelist-names</summary>
        public SvnChangeListCollection ChangeLists =>
            _changelists ?? (_changelists = new SvnChangeListCollection());
    }
}
