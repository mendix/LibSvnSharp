using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.Add(string, SvnAddArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnAddArgs : SvnClientArgs
    {
        SvnDepth _depth;

        public SvnAddArgs()
        {
            _depth = SvnDepth.Infinity;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.Add;

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        public bool NoIgnore { get; set; }

        public bool NoAutoProps { get; set; }

        /// <summary>If force is not set and path is already under version control, return the error
        /// SVN_ERR_ENTRY_EXISTS. If force is set, do not error on already-versioned items. When used
        /// on a directory in conjunction with the recursive flag, this has the effect of scheduling
        /// for addition unversioned files and directories scattered deep within a versioned tree.</summary>
        public bool Force { get; set; }

        /// <summary>
        /// If TRUE, recurse up path's directory and look for
        /// a versioned directory.  If found, add all intermediate paths between it
        /// and path.  If not found, fail with SVN_ERR_CLIENT_NO_VERSIONED_PARENTS.
        /// </summary>
        public bool AddParents { get; set; }
    }
}
