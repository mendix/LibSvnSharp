namespace LibSvnSharp
{
    public sealed class SvnProgressEventArgs : SvnEventArgs
    {
        public SvnProgressEventArgs(long progress, long totalProgress)
        {
            Progress = progress;
            TotalProgress = totalProgress;
        }

        public long Progress { get; }

        public long TotalProgress { get; }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return Progress.GetHashCode();
        }
    }
}
