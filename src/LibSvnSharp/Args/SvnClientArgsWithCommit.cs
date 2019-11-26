using System;
using System.Diagnostics;

namespace LibSvnSharp
{
    /// <summary>Base class of all <see cref="SvnClient" /> arguments which allow committing</summary>
    /// <threadsafety static="true" instance="false"/>
    public abstract class SvnClientArgsWithCommit : SvnClientArgs
    {
        SvnRevisionPropertyCollection _extraProperties;

        protected SvnClientArgsWithCommit()
        {
        }

        /// <summary>Raised just before committing to allow modifying the log message</summary>
        public event EventHandler<SvnCommittingEventArgs> Committing;

        /// <summary>Raised after a successfull commit</summary>
        public event EventHandler<SvnCommittedEventArgs> Committed;

        /// <summary>Applies the <see cref="LogMessage" /> and raises the <see cref="Committing" /> event</summary>
        protected virtual void OnCommitting(SvnCommittingEventArgs e)
        {
            if (LogMessage != null && e.LogMessage == null)
                e.LogMessage = LogMessage;

            Committing?.Invoke(this, e);
        }

        protected virtual void OnCommitted(SvnCommittedEventArgs e)
        {
            Committed?.Invoke(this, e);
        }

        internal void RaiseOnCommitting(SvnCommittingEventArgs e)
        {
            OnCommitting(e);
        }

        internal void RaiseOnCommitted(SvnCommittedEventArgs e)
        {
            OnCommitted(e);
        }

        /// <summary>Gets or sets the logmessage applied to the commit</summary>
        public string LogMessage { get; set; }

        /// <summary>Gets a list of 'extra' revision properties to set on commit</summary>
        public SvnRevisionPropertyCollection LogProperties
        {
            [DebuggerStepThrough]
            get
            {
                if (_extraProperties == null)
                    _extraProperties = new SvnRevisionPropertyCollection();

                return _extraProperties;
            }
        }
    }
}
