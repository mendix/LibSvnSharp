using System;
using System.Runtime.Serialization;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    [Serializable]
    public sealed class SvnMalfunctionException : SvnException
    {
        public SvnMalfunctionException()
        {
        }

        public SvnMalfunctionException(string message)
            : base(message)
        {
        }

        public SvnMalfunctionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public SvnMalfunctionException(string message, string file, int line)
            : base(string.Format(SharpSvnStrings.SvnMalfunctionPrefix, message, file, line), file, line)
        {
        }

        internal SvnMalfunctionException(svn_error_t err)
            : base(err)
        {
        }

        SvnMalfunctionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
