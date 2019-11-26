using System;
using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container for SvnClient.Status</summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnStatusArgs : SvnClientArgs
    {
        SvnDepth _depth;
        SvnRevision _revision;
        SvnChangeListCollection _changelists;

        public event EventHandler<SvnStatusEventArgs> Status;

        protected internal virtual void OnStatus(SvnStatusEventArgs e)
        {
            Status?.Invoke(this, e);
        }

        public SvnStatusArgs()
        {
            _depth = SvnDepth.Infinity;
            _revision = SvnRevision.None;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.Status;

        public SvnRevision Revision
        {
            get => _revision;
            set
            {
                if (value != null)
                    _revision = value;
                else
                    _revision = SvnRevision.None;
            }
        }

        public SvnDepth Depth
        {
            get => _depth;
            set => _depth = EnumVerifier.Verify(value);
        }

        /// <summary>Gets or sets a boolean indicating whether all status properties should be retrieved</summary>
        /// <remarks>
        /// If @a get_all is set, retrieve all entries; otherwise,
        /// retrieve only "interesting" entries (local mods and/or
        /// out of date
        /// </remarks>
        public bool RetrieveAllEntries { get; set; }

        /// <summary>Gets or sets a boolean indicating whether the repository should be contacted to retrieve out of date information</summary>
        /// <remarks>
        /// If Update is set, contact the repository and augment the
        /// status structures with information about out-of-dateness (with
        /// respect to @a revision).  Also, if @a result_rev is not @c NULL,
        /// set @a *result_rev to the actual revision against which the
        /// working copy was compared (result_rev is not meaningful unless
        /// update is set
        /// </remarks>
        public bool RetrieveRemoteStatus { get; set; }

        public bool IgnoreWorkingCopyStatus { get; set; }

        [Obsolete("Please use .RetrieveRemoteStatus instead")]
        public bool ContactRepository
        {
            get => RetrieveRemoteStatus;
            set => RetrieveRemoteStatus = value;
        }

        /// <summary>Gets or sets a boolean indicating whether ignored files should be retrieved</summary>
        /// <remarks>If RetrieveIgnoredEntries is set add files or directories that match ignore patterns.</remarks>
        public bool RetrieveIgnoredEntries { get; set; }

        /// <summary>Gets or sets a boolean indicating whether externals should be ignored</summary>
        /// <remarks>
        /// If IgnoreExternals is not set, then recurse into externals
        /// definitions (if any exist) after handling the main target.  This
        /// calls the client notification function (in @a ctx) with the @c
        /// svn_wc_notify_status_external action before handling each externals
        /// definition, and with @c svn_wc_notify_status_completed
        /// after each.
        /// </remarks>
        public bool IgnoreExternals { get; set; }

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

        /// <summary>If this value is TRUE and RetrieveRemoteStatus it TRUE, shows what an update with KeepDepth TRUE would do. (Shows excluded nodes as additions)</summary>
        public bool KeepDepth { get; set; }
    }
}
