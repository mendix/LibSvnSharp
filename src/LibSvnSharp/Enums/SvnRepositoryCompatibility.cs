namespace LibSvnSharp
{
    public enum SvnRepositoryCompatibility
    {
        /// <summary>Create a repository with the latest available format</summary>
        Default = 0,
        /// <summary>Create a repository in a Subversion 1.0-1.3 compatible format</summary>
        Subversion10,
        /// <summary>Create a repository in Subversion 1.4 compatible format</summary>
        Subversion14,
        /// <summary>Create a repository in Subversion 1.5 compatible format</summary>
        Subversion15,
        /// <summary>Create a repository in Subversion 1.6 compatible format</summary>
        Subversion16,
        /// <summary>Create a repository in Subversion 1.7 compatible format</summary>
        Subversion17,
        /// <summary>Create a repository in Subversion 1.8 compatible format</summary>
        Subversion18,
        /// <summary>Create a repository in Subversion 1.9 compatible format</summary>
        Subversion19,
    }
}
