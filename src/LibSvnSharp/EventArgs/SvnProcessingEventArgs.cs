namespace LibSvnSharp
{
    public sealed class SvnProcessingEventArgs : SvnEventArgs
    {
        internal SvnProcessingEventArgs(SvnCommandType commandType)
        {
            CommandType = commandType;
        }

        public SvnCommandType CommandType { get; }

        /// <summary>Serves as a hashcode for the specified type</summary>
        public override int GetHashCode()
        {
            return CommandType.GetHashCode();
        }
    }
}
