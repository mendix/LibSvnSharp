using System;

namespace LibSvnSharp.Remote
{
    public interface ISvnRepositoryListItem
    {
        Uri Uri { get; }

        SvnDirEntry Entry { get; }
    }
}
