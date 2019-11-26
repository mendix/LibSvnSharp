using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.Export(SvnTarget, string, SvnExportArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnExportArgs : SvnClientArgs
    {
        SvnDepth _depth;
        SvnRevision _revision;
        SvnLineStyle _lineStyle;

        public SvnExportArgs()
        {
            _depth = SvnDepth.Infinity;
            _revision = SvnRevision.None;
        }

        public sealed override SvnCommandType CommandType => SvnCommandType.Export;

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        public bool IgnoreExternals { get; set; }

        public bool Overwrite { get; set; }

        public SvnRevision Revision
        {
            get => _revision;
            set => _revision = value ?? SvnRevision.None;
        }

        public SvnLineStyle LineStyle
        {
            get => _lineStyle;
            set => _lineStyle = EnumVerifier.Verify(value);
        }

        public bool IgnoreKeywords { get; set; }
    }
}
