using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.SetProperty(string,string,string,SvnSetPropertyArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnSetPropertyArgs : SvnClientArgsWithCommit
    {
        SvnDepth _depth;
        long _baseRevision;
        SvnChangeListCollection _changelists;

        public SvnSetPropertyArgs()
        {
            _depth = SvnDepth.Empty;
            _baseRevision = Constants.SVN_INVALID_REVNUM;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.SetProperty;

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        public bool SkipChecks { get; set; }

        public long BaseRevision
        {
            get => _baseRevision;
            set
            {
                if (value >= 0)
                    _baseRevision = value;
                else
                    _baseRevision = Constants.SVN_INVALID_REVNUM;
            }
        }

        /// <summary>Gets the list of changelist-names</summary>
        public SvnChangeListCollection ChangeLists
        {
            get
            {
                if (_changelists == null)
                    _changelists = new SvnChangeListCollection();
                return _changelists;
            }
        }
    }
}
