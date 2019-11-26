using System;
using System.Runtime.Serialization;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [Serializable]
    public class SvnSystemException : SvnException
    {
        internal SvnSystemException(svn_error_t error)
            : base(error)
        {
        }

        protected SvnSystemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SvnSystemException()
        {
        }

        public SvnSystemException(string message)
            : base(message)
        {
        }

        public SvnSystemException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
