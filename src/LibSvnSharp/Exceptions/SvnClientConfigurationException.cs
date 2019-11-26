using System;
using System.Runtime.Serialization;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [Serializable]
    public class SvnClientConfigurationException : SvnClientException
    {
        internal SvnClientConfigurationException(svn_error_t error)
            : base(error)
        { }

        protected SvnClientConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public SvnClientConfigurationException()
        { }

        public SvnClientConfigurationException(string message)
            : base(message)
        { }

        public SvnClientConfigurationException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
