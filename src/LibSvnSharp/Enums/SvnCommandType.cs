namespace LibSvnSharp
{
    public enum SvnCommandType
    {
        Unknown = 0,

        // Add new members at the end
        Add,
        AddToChangeList,
        Blame,
        CheckOut,
        CleanUp,
        Commit,
        Copy,
        CreateDirectory,
        Delete,
        Diff,
        DiffMerge,
        DiffSummary,
        Export,
        GetAppliedMergeInfo,
        GetProperty,
        GetRevisionProperty,
        GetSuggestedMergeSources,
        Import,
        Info,
        List,
        ListChangeList,
        Lock,
        Log,
        Merge,
        MergesEligible,
        MergesMerged,
        Move,
        PropertyList,
        ReintegrationMerge,
        Relocate,
        RemoveFromChangeList,
        Resolved,
        Revert,
        RevisionPropertyList,
        SetProperty,
        SetRevisionProperty,
        Status,
        Switch,
        Unlock,
        Update,
        Write,
        Upgrade,
        Patch,
        InheritedPropertyList,
        RepositoryOperations,
        Youngest,
        Vacuum,

        CropWorkingCopy = 0x501,

        // Wc library helper
        GetWorkingCopyInfo = 0x1001,
        GetWorkingCopyVersion,
        GetWorkingCopyEntries,
        WorkingCopyMove,
        WorkingCopyCopy,
        WorkingCopyRestore,

        // Custom commands
        FileVersions = 0x2001,
        ReplayRevision,
        WriteRelated
    }
}
