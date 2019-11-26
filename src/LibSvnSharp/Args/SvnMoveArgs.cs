using System;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.Move(string,string,SvnMoveArgs)" /> and
    /// <see cref="SvnClient.RemoteMove(Uri,Uri,SvnMoveArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnMoveArgs : SvnClientArgsWithCommit
    {
        bool _noMixedRevisions;

        public override sealed SvnCommandType CommandType => SvnCommandType.Move;

        public bool Force { get; set; }

        public bool AlwaysMoveAsChild { get; set; }

        /// <summary>Creates parent directories if required</summary>
        public bool CreateParents { get; set; }

        public bool AllowMixedRevisions
        {
            get => !_noMixedRevisions;
            set => _noMixedRevisions = !value;
        }

        public bool MetaDataOnly { get; set; }

        [Obsolete("Use .CreateParents")]
        public bool MakeParents
        {
            get => CreateParents;
            set => CreateParents = value;
        }
    }
}
