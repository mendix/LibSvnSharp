using System;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.Delete(String,SvnDeleteArgs)" /> and <see cref="SvnClient.RemoteDelete(Uri,SvnDeleteArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnDeleteArgs : SvnClientArgsWithCommit
    {
        public override sealed SvnCommandType CommandType => SvnCommandType.Delete;

        /// <summary>If Force is not set then this operation will fail if any path contains locally modified
        /// and/or unversioned items. If Force is set such items will be deleted.</summary>
        public bool Force { get; set; }

        public bool KeepLocal { get; set; }
    }
}
