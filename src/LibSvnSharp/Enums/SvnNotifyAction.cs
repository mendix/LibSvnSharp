using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    public enum SvnNotifyAction : uint
    {
        Add                                                     = svn_wc_notify_action_t.svn_wc_notify_add, // =0
        Copy                                                    = svn_wc_notify_action_t.svn_wc_notify_copy,
        Delete                                                  = svn_wc_notify_action_t.svn_wc_notify_delete,
        Restore                                                 = svn_wc_notify_action_t.svn_wc_notify_restore,
        Revert                                                  = svn_wc_notify_action_t.svn_wc_notify_revert,
        RevertFailed                                            = svn_wc_notify_action_t.svn_wc_notify_failed_revert,
        Resolved                                                = svn_wc_notify_action_t.svn_wc_notify_resolved,
        Skip                                                    = svn_wc_notify_action_t.svn_wc_notify_skip,
        UpdateDelete                                            = svn_wc_notify_action_t.svn_wc_notify_update_delete,
        UpdateAdd                                               = svn_wc_notify_action_t.svn_wc_notify_update_add,
        UpdateUpdate                                            = svn_wc_notify_action_t.svn_wc_notify_update_update,
        UpdateCompleted                                         = svn_wc_notify_action_t.svn_wc_notify_update_completed,
        UpdateExternal                                          = svn_wc_notify_action_t.svn_wc_notify_update_external,
        StatusCompleted                                         = svn_wc_notify_action_t.svn_wc_notify_status_completed,
        StatusExternal                                          = svn_wc_notify_action_t.svn_wc_notify_status_external,
        CommitModified                                          = svn_wc_notify_action_t.svn_wc_notify_commit_modified,
        CommitAdded                                             = svn_wc_notify_action_t.svn_wc_notify_commit_added,
        CommitDeleted                                           = svn_wc_notify_action_t.svn_wc_notify_commit_deleted,
        CommitReplaced                                          = svn_wc_notify_action_t.svn_wc_notify_commit_replaced,
        CommitSendData                                          = svn_wc_notify_action_t.svn_wc_notify_commit_postfix_txdelta,
        BlameRevision                                           = svn_wc_notify_action_t.svn_wc_notify_blame_revision,
        // 1.2+
        LockLocked                                              = svn_wc_notify_action_t.svn_wc_notify_locked,
        LockUnlocked                                            = svn_wc_notify_action_t.svn_wc_notify_unlocked,
        LockFailedLock                                          = svn_wc_notify_action_t.svn_wc_notify_failed_lock,
        LockFailedUnlock                                        = svn_wc_notify_action_t.svn_wc_notify_failed_unlock,

        // 1.5+
        Exists                                                  = svn_wc_notify_action_t.svn_wc_notify_exists,
        ChangeListSet                                           = svn_wc_notify_action_t.svn_wc_notify_changelist_set,
        ChangeListClear                                         = svn_wc_notify_action_t.svn_wc_notify_changelist_clear,
        ChangeListMoved                                         = svn_wc_notify_action_t.svn_wc_notify_changelist_moved,
        MergeBegin                                              = svn_wc_notify_action_t.svn_wc_notify_merge_begin,
        MergeBeginForeign                                       = svn_wc_notify_action_t.svn_wc_notify_foreign_merge_begin,
        UpdateReplace                                           = svn_wc_notify_action_t.svn_wc_notify_update_replace,

        // 1.6+
        PropertyAdded                                           = svn_wc_notify_action_t.svn_wc_notify_property_added,
        PropertyModified                                        = svn_wc_notify_action_t.svn_wc_notify_property_modified,
        PropertyDeleted                                         = svn_wc_notify_action_t.svn_wc_notify_property_deleted,
        PropertyDeletedNonExistent                              = svn_wc_notify_action_t.svn_wc_notify_property_deleted_nonexistent,
        RevisionPropertySet                                     = svn_wc_notify_action_t.svn_wc_notify_revprop_set,
        RevisionPropertyDeleted                                 = svn_wc_notify_action_t.svn_wc_notify_revprop_deleted,
        MergeCompleted                                          = svn_wc_notify_action_t.svn_wc_notify_merge_completed,
        TreeConflict                                            = svn_wc_notify_action_t.svn_wc_notify_tree_conflict,
        ExternalFailed                                          = svn_wc_notify_action_t.svn_wc_notify_failed_external,

        // 1.7+
        /// <summary>Starting an update operation.</summary>
        UpdateStarted               = svn_wc_notify_action_t.svn_wc_notify_update_started,

        /// <summary>An update tried to add a file or directory at a path where a separate working copy was found</summary>
        UpdateSkipObstruction       = svn_wc_notify_action_t.svn_wc_notify_update_skip_obstruction,

        /// <summary>An explicit update tried to update a file or directory that doesn't live in the repository and can't be brought in.</summary>
        UpdateSkipWorkingOnly       = svn_wc_notify_action_t.svn_wc_notify_update_skip_working_only,

        /// <summary>An update tried to update a file or directory to which access could not be obtained.</summary>
        UpdateSkipAccessDenied      = svn_wc_notify_action_t.svn_wc_notify_update_skip_access_denied,

        /// <summary>An update operation removed an external working copy.</summary>
        UpdateExternalRemoved       =  svn_wc_notify_action_t.svn_wc_notify_update_external_removed,

        /// <summary>A node below an existing node was added during update.</summary>
        UpdateShadowedAdd           = svn_wc_notify_action_t.svn_wc_notify_update_shadowed_add,

        /// <summary>A node below an exising node was updated during update.</summary>
        UpdateShadowedUpdate        = svn_wc_notify_action_t.svn_wc_notify_update_shadowed_update,

        /// <summary>A node below an existing node was deleted during update.</summary>
        UpdateShadowedDelete        = svn_wc_notify_action_t.svn_wc_notify_update_shadowed_delete,

        /// <summary>The mergeinfo on path was updated.</summary>
        RecordMergeInfo             = svn_wc_notify_action_t.svn_wc_notify_merge_record_info,

        /// <summary>An working copy directory was upgraded to the latest format.</summary>
        UpgradedDirectory           = svn_wc_notify_action_t.svn_wc_notify_upgraded_path,

        /// <summary>Mergeinfo describing a merge was recorded.</summary>
        RecordMergeInfoStarted      = svn_wc_notify_action_t.svn_wc_notify_merge_record_info_begin,

        /// <summary>Mergeinfo was removed due to elision.</summary>
        RecordMergeInfoElided       = svn_wc_notify_action_t.svn_wc_notify_merge_elide_info,

        /// <summary>A file in the working copy was patched.</summary>
        PatchApplied                = svn_wc_notify_action_t.svn_wc_notify_patch,

        /// <summary>A hunk from a patch was applied.</summary>
        PatchAppliedHunk            = svn_wc_notify_action_t.svn_wc_notify_patch_applied_hunk,

        /// <summary>A hunk from a patch was rejected.</summary>
        PatchRejectedHunk           = svn_wc_notify_action_t.svn_wc_notify_patch_rejected_hunk,

        /// <summary>A hunk from a patch was found to already be applied.</summary>
        PatchFoundAlreadyApplied    = svn_wc_notify_action_t.svn_wc_notify_patch_hunk_already_applied,

        /// <summary>Committing a non-overwriting copy (path is the target of the
        /// copy, not the source).</summary>
        CommitAddCopy               = svn_wc_notify_action_t.svn_wc_notify_commit_copied,

        /// <summary>Committing an overwriting (replace) copy (path is the target of
        /// the copy, not the source).</summary>
        CommitReplacedWithCopy      = svn_wc_notify_action_t.svn_wc_notify_commit_copied_replaced,

        /// <summary>The server has instructed the client to follow a URL redirection.</summary>
        FollowUrlRedirect           = svn_wc_notify_action_t.svn_wc_notify_url_redirect,

        /// <summary>The operation was attempted on a path which doesn't exist.</summary>
        NonExistentPath             = svn_wc_notify_action_t.svn_wc_notify_path_nonexistent,

        /// <summary>Removing a path by excluding it.</summary>
        Excluded                    = svn_wc_notify_action_t.svn_wc_notify_exclude,

        /// <summary>Operation failed because the node remains in conflict</summary>
        FailedConflict              = svn_wc_notify_action_t.svn_wc_notify_failed_conflict,

        /// <summary>Operation failed because an added node is missing</summary>
        FailedMissing               =  svn_wc_notify_action_t.svn_wc_notify_failed_missing,

        /// <summary>Operation failed because a node is out of date</summary>
        FailedOutOfDate             = svn_wc_notify_action_t.svn_wc_notify_failed_out_of_date,

        /// <summary>Operation failed because an added parent is not selected</summary>
        FailedNoParent              = svn_wc_notify_action_t.svn_wc_notify_failed_no_parent,

        /// <summary>Operation failed because a node is locked by another user and/or working copy</summary>
        FailedLocked                = svn_wc_notify_action_t.svn_wc_notify_failed_locked,

        /// <summary>Operation failed because the operation was forbidden by the server</summary>
        FailedForbiddenByServer     = svn_wc_notify_action_t.svn_wc_notify_failed_forbidden_by_server,

        /// <summary>The operation skipped the path because it was conflicted.</summary>
        SkipConflicted              = svn_wc_notify_action_t.svn_wc_notify_skip_conflicted,

        // 1.8+
        /// <summary>Just the lock on a file was removed during update</summary>
        UpdateBrokenLock                                = svn_wc_notify_action_t.svn_wc_notify_update_broken_lock,

        /// <summary>Operation failed because a node is obstructed</summary>
        FailedObstruction                               = svn_wc_notify_action_t.svn_wc_notify_failed_obstruction,

        /// <summary>Conflict resolver is starting. This can be used by clients to
        /// detect when to display conflict summary information, for example.</summary>
        ConflictResolverStarting                = svn_wc_notify_action_t.svn_wc_notify_conflict_resolver_starting,


        /// <summary>Conflict resolver is done. This can be used by clients to
        /// detect when to display conflict summary information, for example</summary>
        ConflictResolverDone                    = svn_wc_notify_action_t.svn_wc_notify_conflict_resolver_done,

        /// <summary>The current operation left local changes of something that
        /// was deleted. The changes are available on (and below) the notified path
        /// </summary>
        LeftLocalModifications                  = svn_wc_notify_action_t.svn_wc_notify_left_local_modifications,

        /// <summary>A copy from a foreign repository has started</summary>
        ForeignCopyBegin                                = svn_wc_notify_action_t.svn_wc_notify_foreign_copy_begin,

        /// <summary>A move in the working copy has been broken, i.e. degraded into a
        /// copy + delete. The notified path is the move source (the deleted path).
        /// </summary>
        MoveBroken                                              = svn_wc_notify_action_t.svn_wc_notify_move_broken,

        /// <summary>Running cleanup on an external module.</summary>
        CleanupExternal             = svn_wc_notify_action_t.svn_wc_notify_cleanup_external,

        /// <summary>The operation failed because the operation (E.g. commit) is only valid
        /// if the operation includes this path.</summary>
        OperationRequiresTarget     = svn_wc_notify_action_t.svn_wc_notify_failed_requires_target,

        /// <summary>Running info on an external module.</summary>
        InfoExternal                = svn_wc_notify_action_t.svn_wc_notify_info_external,

        /// <summary>Finalizing commit.</summary>
        CommitFinalizing            = svn_wc_notify_action_t.svn_wc_notify_commit_finalizing,
    }
}
