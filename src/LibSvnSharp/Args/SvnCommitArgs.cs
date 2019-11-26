using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.Commit(string, SvnCommitArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnCommitArgs : SvnClientArgsWithCommit
    {
        SvnDepth _depth;
        SvnChangeListCollection _changelists;

        public SvnCommitArgs()
        {
            _depth = SvnDepth.Infinity;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.Commit;

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        public bool KeepLocks { get; set; }

        /// <summary>Gets the list of changelist-names to commit</summary>
        public SvnChangeListCollection ChangeLists
        {
            get
            {
                if (_changelists == null)
                    _changelists = new SvnChangeListCollection();
                return _changelists;
            }
        }

        public bool KeepChangeLists { get; set; }

        public bool IncludeFileExternals { get; set; }

        public bool IncludeDirectoryExternals { get; set; }

        public bool RunTortoiseHooks { get; set; }
    }
}
