using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of <see cref="SvnClient.Update(string, SvnUpdateArgs)" /></summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnUpdateArgs : SvnClientArgsWithConflict
    {
        SvnDepth _depth;
        SvnRevision _revision;

        public SvnUpdateArgs()
        {
            _depth = SvnDepth.Unknown;
            _revision = SvnRevision.None;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.Update;

        /// <summary>Gets or sets the depth of the update</summary>
        /// <remarks>
        /// <para>If Depth is Infinity, update fully recursively. Else if it
        /// is Immediates or Files, update each target and its file
        /// entries, but not its subdirectories.  Else if Empty, update
        /// exactly each target, nonrecursively (essentially, update the
        /// target's properties).</para>
        /// <para>If Depth is Unknown, take the working depth from the specified paths
        /// </para>
        /// </remarks>
        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        /// <summary>
        /// Gets or sets whether in addition to updating paths also the
        /// sticky ambient depth value must be set to depth
        /// </summary>
        public bool KeepDepth { get; set; }

        /// <summary>Gets or sets a value indicating whether to ignore externals
        /// definitions as part of this operation.</summary>
        public bool IgnoreExternals { get; set; }

        /// <summary>The revision to update to; None by default</summary>
        /// <remarks>
        /// Revision must be of kind Number, Head or Date. If Revision
        /// does not meet these requirements, Updated returns the error
        /// SVN_ERR_CLIENT_BAD_REVISION.
        /// </remarks>
        public SvnRevision Revision
        {
            get => _revision;
            set => _revision = value ?? SvnRevision.None;
        }

        /// <summary>If @a allow_unver_obstructions is <c>true</c> then the update tolerates
        /// existing unversioned items that obstruct added paths from @a URL</summary>
        /// <remarks>
        /// Only obstructions of the same type (file or dir) as the added item are
        /// tolerated.  The text of obstructing files is left as-is, effectively
        /// treating it as a user modification after the update.  Working
        /// properties of obstructing items are set equal to the base properties.
        /// If AllowUnversionedObstructions is <c>false</c> then the update will abort
        /// if there are any unversioned obstructing items.
        /// </remarks>
        public bool AllowObstructions { get; set; }

        public bool AddsAsModifications { get; set; }

        public bool UpdateParents { get; set; }
    }
}
