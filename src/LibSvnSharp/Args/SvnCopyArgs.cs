using System;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of SvnClient.Copy(SvnTarget,String,SvnCopyArgs)" /> and
    /// <see cref="SvnClient.RemoteCopy(SvnTarget,Uri,SvnCopyArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnCopyArgs : SvnClientArgsWithCommit
    {
        SvnRevision _revision;

        public SvnCopyArgs()
        {
            _revision = SvnRevision.None;
        }

        public sealed override SvnCommandType CommandType => SvnCommandType.Copy;

        /// <summary>Creates parent directories if required</summary>
        public bool CreateParents { get; set; }

        [Obsolete("Use .CreateParents")]
        public bool MakeParents
        {
            get => CreateParents;
            set => CreateParents = value;
        }

        /// <summary>Always copies the result to below the target (this behaviour is always used if multiple targets are provided)</summary>
        public bool AlwaysCopyAsChild { get; set; }

        public bool MetaDataOnly { get; set; }

        public bool PinExternals { get; set; }

        /// <summary>Gets or sets a boolean that if set to true tells copy not to process
        /// externals definitions as part of this operation.</summary>
        public bool IgnoreExternals { get; set; }

        public SvnRevision Revision
        {
            get => _revision;
            set => _revision = value ?? SvnRevision.None;
        }
    }
}
