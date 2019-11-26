using System;
using System.Runtime.Serialization;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [Serializable]
    public class SvnOperationCompletedException : SvnException
    {
        internal SvnOperationCompletedException(svn_error_t error)
            : base(error)
        {
        }

        protected SvnOperationCompletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SvnOperationCompletedException()
        {
        }

        public SvnOperationCompletedException(string message)
            : base(message)
        {
        }

        public SvnOperationCompletedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
