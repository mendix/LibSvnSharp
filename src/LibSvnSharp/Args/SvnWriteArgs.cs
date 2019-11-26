using System.IO;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.Write(SvnTarget, Stream, SvnWriteArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnWriteArgs : SvnClientArgs
    {
        SvnRevision _revision;

        public SvnWriteArgs()
        {
            _revision = SvnRevision.None;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.Write;

        public SvnRevision Revision
        {
            get => _revision;
            set => _revision = value ?? SvnRevision.None;
        }

        public bool IgnoreKeywords { get; set; }
    }
}
