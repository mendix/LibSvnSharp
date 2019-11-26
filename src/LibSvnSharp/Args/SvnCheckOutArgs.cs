using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.CheckOut(SvnUriTarget, string, SvnCheckOutArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnCheckOutArgs : SvnClientArgs
    {
        SvnDepth _depth;
        SvnRevision _revision;

        public SvnCheckOutArgs()
        {
            _depth = SvnDepth.Unknown;
            _revision = SvnRevision.None;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.CheckOut;

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        public bool IgnoreExternals { get; set; }

        /// <summary>Gets or sets the revision to load. Defaults to the peg revision or Head</summary>
        public SvnRevision Revision
        {
            get => _revision;
            set => _revision = value ?? SvnRevision.None;
        }

        /// <summary>Gets or sets the AllowObsstructions value</summary>
        /// <remarks>
        /// <para> If AllowObstructions is TRUE then the update tolerates
        /// existing unversioned items that obstruct added paths from @a URL.  Only
        /// obstructions of the same type (file or dir) as the added item are
        /// tolerated.  The text of obstructing files is left as-is, effectively
        /// treating it as a user modification after the update.  Working
        /// properties of obstructing items are set equal to the base properties.</para>
        /// <para>If AllowObstructions is FALSE then the update will abort
        /// if there are any unversioned obstructing items
        /// </para>
        /// </remarks>
        public bool AllowObstructions { get; set; }
    }
}
