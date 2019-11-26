using System;

namespace LibSvnSharp
{
    public class SvnErrorEventArgs : SvnCancelEventArgs
    {
        public SvnErrorEventArgs(SvnException exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public SvnException Exception { get; }
    }
}
