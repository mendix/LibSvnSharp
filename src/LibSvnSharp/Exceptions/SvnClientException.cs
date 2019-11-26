using System;
using System.Runtime.Serialization;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [Serializable]
    public class SvnClientException : SvnException
    {
        internal SvnClientException(svn_error_t error)
            : base(error)
        { }

        protected SvnClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public SvnClientException()
        { }

        public SvnClientException(string message)
            : base(message)
        { }

        public SvnClientException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
