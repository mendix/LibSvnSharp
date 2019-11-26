using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient" />'s GetProperty</summary>
    public class SvnGetPropertyArgs : SvnClientArgs
    {
        SvnRevision _revision;
        SvnDepth _depth;
        SvnChangeListCollection _changelists;

        public SvnGetPropertyArgs()
        {
            _depth = SvnDepth.Empty;
            _revision = SvnRevision.None;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.GetProperty;

        public SvnRevision Revision
        {
            get => _revision;
            set => _revision = value ?? SvnRevision.None;
        }

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        /// <summary>Gets the list of changelist-names</summary>
        public SvnChangeListCollection ChangeLists => _changelists ?? (_changelists = new SvnChangeListCollection());
    }
}
