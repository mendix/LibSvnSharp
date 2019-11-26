using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    public abstract class SvnEventArgs : System.EventArgs
    {
        static SvnEventArgs()
        {
            SvnBase.EnsureLoaded();
        }

        protected SvnEventArgs()
        {
        }

        /// <summary>Detaches the SvnEventArgs from the unmanaged storage; optionally keeping the property values for later use</summary>
        /// <description>After this method is called all properties values are stored in managed code</description>
        public void Detach()
        {
            Detach(true);
        }

        /// <summary>Detaches the SvnEventArgs from the unmanaged storage; optionally keeping the property values for later use</summary>
        /// <description>After this method is called all properties are either stored managed, or are no longer readable</description>
        protected internal virtual void Detach(bool keepProperties)
        {
        }

        internal static int SafeGetHashCode(object value)
        {
            return value?.GetHashCode() ?? 0;
        }
    }
}
