using System;

namespace LibSvnSharp
{
    /// <summary>Base class of all <see cref="SvnClient" /> arguments which allow handling conflicts</summary>
    /// <threadsafety static="true" instance="false"/>
    public abstract class SvnClientArgsWithConflict : SvnClientArgs
    {
        protected SvnClientArgsWithConflict()
        {
        }

        /// <summary>
        /// Raised on conflict. The event is first
        /// raised on the <see cref="SvnClientArgsWithConflict" /> object and
        /// then on the <see cref="SvnClient" />
        /// </summary>
        public EventHandler<SvnConflictEventArgs> Conflict;

        /// <summary>Raises the <see cref="Conflict" /> event</summary>
        protected virtual void OnConflict(SvnConflictEventArgs e)
        {
            Conflict?.Invoke(this, e);
        }

        internal void RaiseConflict(SvnConflictEventArgs e)
        {
            OnConflict(e);
        }
    }
}
