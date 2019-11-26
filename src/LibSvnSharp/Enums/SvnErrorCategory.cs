namespace LibSvnSharp
{
    /// <summary>Gets the error category of the SvnException</summary>
    public enum SvnErrorCategory
    {
        OperatingSystem = -1,
        None = 0,
        Bad = 1,
        Xml = 2,
        IO = 3,
        Stream = 4,
        Node = 5,
        Entry = 6,
        WorkingCopy = 7,
        FileSystem = 8,
        Repository = 9,
        RepositoryAccess = 10,
        RepositoryAccessDav = 11,
        RepositoryAccessLocal = 12,
        SvnDiff = 13,
        ApacheModule = 14,
        Client = 15,
        Misc = 16,
        CommandLine = 17,
        RepositoryAccessSvn = 18,
        Authentication = 19,
        Authorization = 20,
        Diff = 21,
        RepositoryAccessSerf = 22,
        Malfunction = 23,
    }
}
