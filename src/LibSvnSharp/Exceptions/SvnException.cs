using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    [Serializable]
    public class SvnException : Exception
    {
        const string MANAGED_EXCEPTION_PREFIX = "Forwarded Managed Inner Exception/LibSvnSharp/Handle=";
        const int _abusedErrorCode = (int) SvnErrorCode.SVN_ERR_TEST_FAILED; // Used for plugging in managed exceptions; this ID is an implementation detail

        readonly int _errorCode;
        [NonSerialized] unsafe sbyte* _pFile;
        string _file;
        readonly long _line;
        [NonSerialized] object _targets;

        static unsafe string GetErrorText(svn_error_t error)
        {
            if (error == null)
                return "";

            try
            {
                if (error.message != null)
                    return SvnBase.Utf8_PtrToString(error.message);

                var buffer = new sbyte[1024];
                string msg;

                fixed (sbyte* buf = &buffer[0])
                {
                    svn_error.svn_err_best_message(error, buf, Convert.ToUInt64(buffer.Length) - 1);

                    msg = SvnBase.Utf8_PtrToString(buf);
                }

                return msg?.Trim();
            }
            catch (Exception)
            {
                return "Subversion error: Unable to retrieve error text";
            }
        }

        static Exception GetInnerException(svn_error_t error)
        {
            return error?.child != null ? Create(error.child, false) : null;
        }

        internal static SvnException Create(svn_error_t error)
        {
            return (SvnException) Create(error, true);
        }

        internal static unsafe Exception Create(svn_error_t error, bool clearError)
        {
            if (error == null)
                return null;

            int prefixLength = MANAGED_EXCEPTION_PREFIX.Length;

            if (error.apr_err == _abusedErrorCode)
            {
                if (error.message != null)
                {
                    var errorMessage = new string(error.message);
                    if (errorMessage.StartsWith(MANAGED_EXCEPTION_PREFIX))
                    {
                        try
                        {
                            var handleString = errorMessage.Substring(prefixLength);

                            if (long.TryParse(handleString, out var containerHandle))
                            {
                                var container = SvnExceptionContainer.Get(new IntPtr(containerHandle));
                                return SvnExceptionContainer.Fetch(container);
                            }
                        }
                        catch
                        {
                            // suppress
                        }
                    }
                }
            }

            try
            {
                switch(error.apr_err)
                {
                /*
                case (int) SvnErrorCode.SVN_ERR_BAD_FILENAME:
                case (int) SvnErrorCode.SVN_ERR_BAD_URL:
                case (int) SvnErrorCode.SVN_ERR_BAD_DATE:
                case (int) SvnErrorCode.SVN_ERR_BAD_MIME_TYPE:
                case (int) SvnErrorCode.SVN_ERR_BAD_PROPERTY_VALUE:
                case (int) SvnErrorCode.SVN_ERR_BAD_VERSION_FILE_FORMAT:
                case (int) SvnErrorCode.SVN_ERR_BAD_RELATIVE_PATH:
                case (int) SvnErrorCode.SVN_ERR_BAD_UUID:
                case (int) SvnErrorCode.SVN_ERR_BAD_CONFIG_VALUE:
                    return new SvnFormatException(error);
                case (int) SvnErrorCode.SVN_ERR_XML_ATTRIB_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_XML_MISSING_ANCESTRY:
                case (int) SvnErrorCode.SVN_ERR_XML_UNKNOWN_ENCODING:
                case (int) SvnErrorCode.SVN_ERR_XML_MALFORMED:
                case (int) SvnErrorCode.SVN_ERR_XML_UNESCAPABLE_DATA:
                    return new SvnXmlException(error);
                case (int) SvnErrorCode.SVN_ERR_IO_INCONSISTENT_EOL:
                case (int) SvnErrorCode.SVN_ERR_IO_UNKNOWN_EOL:
                case (int) SvnErrorCode.SVN_ERR_IO_UNIQUE_NAMES_EXHAUSTED:
                case (int) SvnErrorCode.SVN_ERR_IO_WRITE_ERROR:
                    return new SvnIOException(error);
                case (int) SvnErrorCode.SVN_ERR_STREAM_UNEXPECTED_EOF:
                case (int) SvnErrorCode.SVN_ERR_STREAM_MALFORMED_DATA:
                case (int) SvnErrorCode.SVN_ERR_STREAM_UNRECOGNIZED_DATA:
                    return new SvnStreamException(error);
                case (int) SvnErrorCode.SVN_ERR_NODE_UNKNOWN_KIND:
                case (int) SvnErrorCode.SVN_ERR_NODE_UNEXPECTED_KIND:
                    return new SvnNodeException(error);
                case (int) SvnErrorCode.SVN_ERR_ENTRY_NOT_FOUND:
                    return new SvnEntryNotFoundException(error);
                case (int) SvnErrorCode.SVN_ERR_ENTRY_EXISTS:
                case (int) SvnErrorCode.SVN_ERR_ENTRY_MISSING_REVISION:
                case (int) SvnErrorCode.SVN_ERR_ENTRY_MISSING_URL:
                case (int) SvnErrorCode.SVN_ERR_ENTRY_ATTRIBUTE_INVALID:
                    return new SvnEntryException(error);
                case (int) SvnErrorCode.SVN_ERR_WC_OBSTRUCTED_UPDATE:
                    return new SvnObstructedUpdateException(error);
                case (int) SvnErrorCode.SVN_ERR_WC_NOT_DIRECTORY:
                case (int) SvnErrorCode.SVN_ERR_WC_NOT_FILE:
                    return new SvnInvalidNodeKindException(error);
                case (int) SvnErrorCode.SVN_ERR_WC_BAD_ADM_LOG:
                case (int) SvnErrorCode.SVN_ERR_WC_NOT_UP_TO_DATE:
                case (int) SvnErrorCode.SVN_ERR_WC_LEFT_LOCAL_MOD:
                case (int) SvnErrorCode.SVN_ERR_WC_SCHEDULE_CONFLICT:
                case (int) SvnErrorCode.SVN_ERR_WC_PATH_FOUND:
                case (int) SvnErrorCode.SVN_ERR_WC_FOUND_CONFLICT:
                case (int) SvnErrorCode.SVN_ERR_WC_CORRUPT:
                case (int) SvnErrorCode.SVN_ERR_WC_CORRUPT_TEXT_BASE:
                case (int) SvnErrorCode.SVN_ERR_WC_NODE_KIND_CHANGE:
                case (int) SvnErrorCode.SVN_ERR_WC_INVALID_OP_ON_CWD:
                case (int) SvnErrorCode.SVN_ERR_WC_BAD_ADM_LOG_START:
                case (int) SvnErrorCode.SVN_ERR_WC_UNSUPPORTED_FORMAT:
                case (int) SvnErrorCode.SVN_ERR_WC_BAD_PATH:
                case (int) SvnErrorCode.SVN_ERR_WC_INVALID_SCHEDULE:
                case (int) SvnErrorCode.SVN_ERR_WC_INVALID_RELOCATION:
                case (int) SvnErrorCode.SVN_ERR_WC_INVALID_SWITCH:
                case (int) SvnErrorCode.SVN_ERR_WC_MISMATCHED_CHANGELIST:
                case (int) SvnErrorCode.SVN_ERR_WC_CONFLICT_RESOLVER_FAILURE:
                case (int) SvnErrorCode.SVN_ERR_WC_COPYFROM_PATH_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_WC_CHANGELIST_MOVE:
                case (int) SvnErrorCode.SVN_ERR_WC_CANNOT_DELETE_FILE_EXTERNAL:
                case (int) SvnErrorCode.SVN_ERR_WC_CANNOT_MOVE_FILE_EXTERNAL:
                    return new SvnWorkingCopyException(error);
                case (int) SvnErrorCode.SVN_ERR_WC_PATH_NOT_FOUND:
                    return new SvnWorkingCopyPathNotFoundException(error);
                case (int) SvnErrorCode.SVN_ERR_WC_LOCKED:
                case (int) SvnErrorCode.SVN_ERR_WC_NOT_LOCKED:
                    return new SvnWorkingCopyLockException(error);
                case (int) SvnErrorCode.SVN_ERR_FS_GENERAL:
                case (int) SvnErrorCode.SVN_ERR_FS_CLEANUP:
                case (int) SvnErrorCode.SVN_ERR_FS_ALREADY_OPEN:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_OPEN:
                case (int) SvnErrorCode.SVN_ERR_FS_CORRUPT:
                case (int) SvnErrorCode.SVN_ERR_FS_PATH_SYNTAX:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_REVISION:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_TRANSACTION:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_ENTRY:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_REPRESENTATION:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_STRING:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_COPY:
                case (int) SvnErrorCode.SVN_ERR_FS_TRANSACTION_NOT_MUTABLE:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_FS_ID_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_ID:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_SINGLE_PATH_COMPONENT:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_MUTABLE:
                case (int) SvnErrorCode.SVN_ERR_FS_ALREADY_EXISTS:
                case (int) SvnErrorCode.SVN_ERR_FS_ROOT_DIR:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_TXN_ROOT:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_REVISION_ROOT:
                case (int) SvnErrorCode.SVN_ERR_FS_REP_CHANGED:
                case (int) SvnErrorCode.SVN_ERR_FS_REP_NOT_MUTABLE:
                case (int) SvnErrorCode.SVN_ERR_FS_MALFORMED_SKEL:
                case (int) SvnErrorCode.SVN_ERR_FS_BERKELEY_DB:
                case (int) SvnErrorCode.SVN_ERR_FS_BERKELEY_DB_DEADLOCK:
                case (int) SvnErrorCode.SVN_ERR_FS_TRANSACTION_DEAD:
                case (int) SvnErrorCode.SVN_ERR_FS_TRANSACTION_NOT_DEAD:
                case (int) SvnErrorCode.SVN_ERR_FS_UNKNOWN_FS_TYPE:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_USER:
                case (int) SvnErrorCode.SVN_ERR_FS_UNSUPPORTED_FORMAT:
                case (int) SvnErrorCode.SVN_ERR_FS_REP_BEING_WRITTEN:
                case (int) SvnErrorCode.SVN_ERR_FS_TXN_NAME_TOO_LONG:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_NODE_ORIGIN:
                case (int) SvnErrorCode.SVN_ERR_FS_UNSUPPORTED_UPGRADE:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_CHECKSUM_REP:
                    return new SvnFileSystemException(error);
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_DIRECTORY:
                case (int) SvnErrorCode.SVN_ERR_FS_NOT_FILE:
                    return new SvnFileSystemNodeTypeException(error);
                case (int) SvnErrorCode.SVN_ERR_FS_CONFLICT:
                case (int) SvnErrorCode.SVN_ERR_FS_OUT_OF_DATE:
                case (int) SvnErrorCode.SVN_ERR_FS_TXN_OUT_OF_DATE:
                    return new SvnFileSystemOutOfDateException(error);
                case (int) SvnErrorCode.SVN_ERR_FS_PATH_ALREADY_LOCKED:
                case (int) SvnErrorCode.SVN_ERR_FS_PATH_NOT_LOCKED:
                case (int) SvnErrorCode.SVN_ERR_FS_BAD_LOCK_TOKEN:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_LOCK_TOKEN:
                case (int) SvnErrorCode.SVN_ERR_FS_LOCK_OWNER_MISMATCH:
                case (int) SvnErrorCode.SVN_ERR_FS_NO_SUCH_LOCK:
                case (int) SvnErrorCode.SVN_ERR_FS_LOCK_EXPIRED:
                    return new SvnFileSystemLockException(error);
                case (int) SvnErrorCode.SVN_ERR_REPOS_LOCKED:
                case (int) SvnErrorCode.SVN_ERR_REPOS_BAD_ARGS:
                case (int) SvnErrorCode.SVN_ERR_REPOS_NO_DATA_FOR_REPORT:
                case (int) SvnErrorCode.SVN_ERR_REPOS_BAD_REVISION_REPORT:
                case (int) SvnErrorCode.SVN_ERR_REPOS_UNSUPPORTED_VERSION:
                case (int) SvnErrorCode.SVN_ERR_REPOS_DISABLED_FEATURE:
                case (int) SvnErrorCode.SVN_ERR_REPOS_UNSUPPORTED_UPGRADE:
                    return new SvnRepositoryException(error);
                case (int) SvnErrorCode.SVN_ERR_REPOS_HOOK_FAILURE:
                case (int) SvnErrorCode.SVN_ERR_REPOS_POST_COMMIT_HOOK_FAILED:
                case (int) SvnErrorCode.SVN_ERR_REPOS_POST_LOCK_HOOK_FAILED:
                case (int) SvnErrorCode.SVN_ERR_REPOS_POST_UNLOCK_HOOK_FAILED:
                    return new SvnRepositoryHookException(error);
                case (int) SvnErrorCode.SVN_ERR_RA_ILLEGAL_URL:
                case (int) SvnErrorCode.SVN_ERR_RA_UNKNOWN_AUTH:
                case (int) SvnErrorCode.SVN_ERR_RA_NOT_IMPLEMENTED:
                case (int) SvnErrorCode.SVN_ERR_RA_OUT_OF_DATE:
                case (int) SvnErrorCode.SVN_ERR_RA_NO_REPOS_UUID:
                case (int) SvnErrorCode.SVN_ERR_RA_UNSUPPORTED_ABI_VERSION:
                case (int) SvnErrorCode.SVN_ERR_RA_NOT_LOCKED:
                case (int) SvnErrorCode.SVN_ERR_RA_PARTIAL_REPLAY_NOT_SUPPORTED:
                case (int) SvnErrorCode.SVN_ERR_RA_REPOS_ROOT_URL_MISMATCH:
                    return new SvnRepositoryIOException(error);

                case (int) SvnErrorCode.SVN_ERR_RA_NOT_AUTHORIZED:
                    return new SvnAuthorizationException(error);

                case (int) SvnErrorCode.SVN_ERR_RA_DAV_SOCK_INIT:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_CREATING_REQUEST:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_REQUEST_FAILED:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_OPTIONS_REQ_FAILED:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_PROPS_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_ALREADY_EXISTS:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_INVALID_CONFIG_VALUE:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_PATH_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_PROPPATCH_FAILED:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_MALFORMED_DATA:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_RESPONSE_HEADER_BADNESS:
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_RELOCATED:
                    return new SvnRepositoryIOException(error);
                case (int) SvnErrorCode.SVN_ERR_RA_DAV_FORBIDDEN:
                    return new SvnRepositoryIOForbiddenException(error);
                case (int) SvnErrorCode.SVN_ERR_RA_LOCAL_REPOS_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_RA_LOCAL_REPOS_OPEN_FAILED:
                    return new SvnRepositoryIOException(error);
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_CMD_ERR:
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_UNKNOWN_CMD:
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_CONNECTION_CLOSED:
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_IO_ERROR:
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_MALFORMED_DATA:
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_REPOS_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_BAD_VERSION:
                case (int) SvnErrorCode.SVN_ERR_RA_SVN_NO_MECHANISMS:
                case (int) SvnErrorCode.SVN_ERR_RA_SERF_SSPI_INITIALISATION_FAILED:
                case (int) SvnErrorCode.SVN_ERR_RA_SERF_SSL_CERT_UNTRUSTED:
                    return new SvnRepositoryIOException(error);

                case (int) SvnErrorCode.SVN_ERR_AUTHN_CREDS_UNAVAILABLE:
                case (int) SvnErrorCode.SVN_ERR_AUTHN_NO_PROVIDER:
                case (int) SvnErrorCode.SVN_ERR_AUTHN_PROVIDERS_EXHAUSTED:
                case (int) SvnErrorCode.SVN_ERR_AUTHN_CREDS_NOT_SAVED:
                case (int) SvnErrorCode.SVN_ERR_AUTHN_FAILED:
                    return new SvnAuthenticationException(error);
                case (int) SvnErrorCode.SVN_ERR_AUTHZ_ROOT_UNREADABLE:
                case (int) SvnErrorCode.SVN_ERR_AUTHZ_UNREADABLE:
                case (int) SvnErrorCode.SVN_ERR_AUTHZ_PARTIALLY_READABLE:
                case (int) SvnErrorCode.SVN_ERR_AUTHZ_INVALID_CONFIG:
                case (int) SvnErrorCode.SVN_ERR_AUTHZ_UNWRITABLE:
                    return new SvnAuthorizationException(error);

                case (int) SvnErrorCode.SVN_ERR_SVNDIFF_INVALID_HEADER:
                case (int) SvnErrorCode.SVN_ERR_SVNDIFF_CORRUPT_WINDOW:
                case (int) SvnErrorCode.SVN_ERR_SVNDIFF_BACKWARD_VIEW:
                case (int) SvnErrorCode.SVN_ERR_SVNDIFF_INVALID_OPS:
                case (int) SvnErrorCode.SVN_ERR_SVNDIFF_UNEXPECTED_END:
                case (int) SvnErrorCode.SVN_ERR_SVNDIFF_INVALID_COMPRESSED_DATA:
                    return new SvnDiffException(error);

                case (int) SvnErrorCode.SVN_ERR_DIFF_DATASOURCE_MODIFIED:
                    return new SvnNodeDiffException(error);

                case (int) SvnErrorCode.SVN_ERR_CLIENT_RA_ACCESS_REQUIRED:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_BAD_REVISION:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_DUPLICATE_COMMIT_URL:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_INVALID_EXTERNALS_DESCRIPTION:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_MODIFIED:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_REVISION_RANGE:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_INVALID_RELOCATION:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_REVISION_AUTHOR_CONTAINS_NEWLINE:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_PROPERTY_NAME:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_MULTIPLE_SOURCES_DISALLOWED:
                case (int) SvnErrorCode.SVN_ERR_CLIENT_FILE_EXTERNAL_OVERWRITE_VERSIONED:
                    return new SvnClientApiException(error);

                case (int) SvnErrorCode.SVN_ERR_CLIENT_IS_DIRECTORY:
                    return new SvnClientNodeKindException(error);
                case (int) SvnErrorCode.SVN_ERR_CLIENT_IS_BINARY_FILE:
                    return new SvnClientBinaryFileException(error);

                case (int) SvnErrorCode.SVN_ERR_CLIENT_VERSIONED_PATH_REQUIRED:
                    return new SvnClientNoVersionedPathException(error);
                case (int) SvnErrorCode.SVN_ERR_CLIENT_NO_VERSIONED_PARENT:
                    return new SvnClientNoVersionedParentException(error);
                case (int) SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES:
                    return new SvnClientUnrelatedResourcesException(error);
                case (int) SvnErrorCode.SVN_ERR_CLIENT_MISSING_LOCK_TOKEN:
                    return new SvnClientMissingLockTokenException(error);

                case (int) SvnErrorCode.SVN_ERR_CLIENT_NOT_READY_TO_MERGE:
                    return new SvnClientNotReadyToMergeException(error);

                //case (int) SvnErrorCode.SVN_ERR_MERGE_INFO_PARSE_ERROR:
                //        return new SvnException(error);

                case (int) SvnErrorCode.SVN_ERR_CANCELLED:
                    return new SvnOperationCanceledException(error);
                */
                case (int) SvnErrorCode.SVN_ERR_CEASE_INVOCATION:
                    return new SvnOperationCompletedException(error);
                /*
                case (int) SvnErrorCode.SVN_ERR_ITER_BREAK:
                    return new SvnBreakIterationException(error);

                case (int) SvnErrorCode.SVN_ERR_UNKNOWN_CHANGELIST:
                    return new SvnUnknownChangeListException(error);

                case (int) SvnErrorCode.SVN_ERR_ILLEGAL_TARGET:
                    return new SvnIllegalTargetException(error);

                case (int) SvnErrorCode.SVN_ERR_UNVERSIONED_RESOURCE:
                    return new SvnUnversionedNodeException(error);

                case (int) SvnErrorCode.SVN_ERR_BASE:
                case (int) SvnErrorCode.SVN_ERR_PLUGIN_LOAD_FAILURE:
                case (int) SvnErrorCode.SVN_ERR_MALFORMED_FILE:
                case (int) SvnErrorCode.SVN_ERR_INCOMPLETE_DATA:
                case (int) SvnErrorCode.SVN_ERR_INCORRECT_PARAMS:
                case (int) SvnErrorCode.SVN_ERR_TEST_FAILED:
                case (int) SvnErrorCode.SVN_ERR_BAD_PROP_KIND:
                case (int) SvnErrorCode.SVN_ERR_DELTA_MD5_CHECKSUM_ABSENT:
                case (int) SvnErrorCode.SVN_ERR_DIR_NOT_EMPTY:
                case (int) SvnErrorCode.SVN_ERR_EXTERNAL_PROGRAM:
                case (int) SvnErrorCode.SVN_ERR_SWIG_PY_EXCEPTION_SET:
                case (int) SvnErrorCode.SVN_ERR_INVALID_DIFF_OPTION:
                case (int) SvnErrorCode.SVN_ERR_PROPERTY_NOT_FOUND:
                case (int) SvnErrorCode.SVN_ERR_NO_AUTH_FILE_PATH:
                case (int) SvnErrorCode.SVN_ERR_VERSION_MISMATCH:
                case (int) SvnErrorCode.SVN_ERR_MERGEINFO_PARSE_ERROR:
                case (int) SvnErrorCode.SVN_ERR_REVNUM_PARSE_FAILURE:
                    // TODO: Split out
                    return new SvnException(error);

                case (int) SvnErrorCode.SVN_ERR_UNSUPPORTED_FEATURE:
                    return new SvnUnsupportedFeatureException(error);
                case (int) SvnErrorCode.SVN_ERR_UNKNOWN_CAPABILITY:
                    return new SvnUnknownCapabilityException(error);
                case (int) SvnErrorCode.SVN_ERR_CHECKSUM_MISMATCH:
                    return new SvnChecksumMismatchException(error);
                case (int) SvnErrorCode.SVN_ERR_RESERVED_FILENAME_SPECIFIED:
                    return new SvnReservedNameUsedException(error);
                */

                default:
                    if (error.apr_err >= Constants.SVN_ERR_BAD_CATEGORY_START
                        && error.apr_err < (Constants.SVN_ERR_MALFUNC_CATEGORY_START + Constants.SVN_ERR_CATEGORY_SIZE))
                    {
                        /*
                        if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_BAD_CATEGORY_START)) return new SvnFormatException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_XML_CATEGORY_START)) return new SvnXmlException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_IO_CATEGORY_START)) return new SvnIOException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_STREAM_CATEGORY_START)) return new SvnStreamException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_NODE_CATEGORY_START)) return new SvnNodeException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_ENTRY_CATEGORY_START)) return new SvnEntryException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_WC_CATEGORY_START)) return new SvnWorkingCopyException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_FS_CATEGORY_START)) return new SvnFileSystemException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_REPOS_CATEGORY_START)) return new SvnRepositoryException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_RA_CATEGORY_START)) return new SvnRepositoryIOException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_RA_DAV_CATEGORY_START)) return new SvnRepositoryIOException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_RA_LOCAL_CATEGORY_START)) return new SvnRepositoryIOException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_SVNDIFF_CATEGORY_START)) return new SvnDiffException(error);
                        //else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_APMOD_CATEGORY_START)) return new SvnException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_CLIENT_CATEGORY_START)) return new SvnClientException(error);
                        //else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_MISC_CATEGORY_START)) return new SvnException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_CL_CATEGORY_START)) return new SvnClientException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_RA_SVN_CATEGORY_START)) return new SvnRepositoryIOException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_AUTHN_CATEGORY_START)) return new SvnAuthenticationException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_AUTHZ_CATEGORY_START)) return new SvnAuthorizationException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_DIFF_CATEGORY_START)) return new SvnNodeDiffException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_RA_SERF_CATEGORY_START)) return new SvnRepositoryIOException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_MALFUNC_CATEGORY_START)) return new SvnMalfunctionException(error);
                        else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SVN_ERR_X509_CATEGORY_START)) return new SvnX509Exception(error);
                        //else if (SVN_ERROR_IN_CATEGORY(error.apr_err, SvnSshException::_svnSshExceptionBase)) return new SvnSshException(error);
                        */
                        return new SvnException(error);
                    }
                    /*
                    else if (APR_STATUS_IS_ENOSPC(error.apr_err))
                        return new SvnDiskFullException(error);
                    else if (error.apr_err >= SERF_ERROR_START && error.apr_err < (SERF_ERROR_START + SERF_ERROR_RANGE))
                        return new SvnSerfException(error);
                    */
                    else if (error.apr_err >= Constants.APR_OS_START_SYSERR || error.apr_err < Constants.APR_OS_START_ERROR)
                        return new SvnSystemException(error);
                    else
                        return new SvnException(error);
                }
            }
            finally
            {
                if (clearError)
                    svn_error.svn_error_clear(error);
            }
        }

        internal static svn_error_t CreateExceptionSvnError(string origin, Exception exception)
        {
            svn_error_t innerError = null;

            if (exception != null)
                innerError = CreateSvnError(exception);

            int status = (int) SvnErrorCode.SVN_ERR_CANCELLED;
            if (exception is SvnException se)
                status = se.SubversionErrorCode;

            // Use svn_error_createf to make sure the value is copied
            return svn_error.svn_error_create(
                status,
                innerError,
                $"Operation canceled. Exception occured in {origin}");
        }

        internal static svn_error_t CreateSvnError(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            svn_error_t creator = svn_error.svn_error_create(_abusedErrorCode, null, "{Managed Exception Blob}");

            if (creator.pool != null)
            {
                var ex = new SvnExceptionContainer(exception, creator.pool);

                return svn_error.svn_error_create(_abusedErrorCode, creator, MANAGED_EXCEPTION_PREFIX + ex.Handle.ToString("X"));
            }

            return svn_error.svn_error_create((int) SvnErrorCode.SVN_ERR_BASE, null, null);
        }

        protected unsafe SvnException(svn_error_t error)
            : base(GetErrorText(error), GetInnerException(error))
        {
            if (error != null)
            {
                _errorCode = error.apr_err;
                _line = error.line;
                _pFile = error.file;
            }
        }

        protected SvnException(string message, string file, int line)
            : base(message)
        {
            _file = file;
            _line = line;
        }

        protected SvnException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            _errorCode = info.GetInt32("SvnErrorValue");
            _file = info.GetString("_file");
            _line = info.GetInt32("_line");
        }

        public SvnException()
        {
        }

        public SvnException(string message)
            : base(message)
        {
        }

        public SvnException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>Gets the raw subversion error code</summary>
        public int SubversionErrorCode => _errorCode;

        /// <summary>Gets the operating system error code when there is one
        /// (Only valid if SvnErrorCategory returns <See cref="LibSvnSharp.SvnErrorCategory.OperatingSystem" />)
        /// </summary>
        public int OperatingSystemErrorCode
        {
            get
            {
                if (_errorCode >= Constants.APR_OS_START_SYSERR)
                    return (_errorCode == 0 ? Constants.APR_SUCCESS : _errorCode - Constants.APR_OS_START_SYSERR);

                return 0;
            }
        }

        /// <summary>Gets the raw subversion error code casted to a <see cref="LibSvnSharp.SvnErrorCode" /></summary>
        public SvnErrorCode SvnErrorCode => (SvnErrorCode) SubversionErrorCode;

        public SvnAprErrorCode AprErrorCode => (SvnAprErrorCode) SubversionErrorCode;

        public SvnErrorCategory SvnErrorCategory
        {
            get
            {
                if (_errorCode >= Constants.SVN_ERR_BAD_CATEGORY_START && _errorCode < Constants.SVN_ERR_RA_SERF_CATEGORY_START + Constants.SVN_ERR_CATEGORY_SIZE)
                    return (SvnErrorCategory) (_errorCode / Constants.SVN_ERR_CATEGORY_SIZE);
                if (_errorCode >= Constants.APR_OS_START_SYSERR)
                    return SvnErrorCategory.OperatingSystem;
                return SvnErrorCategory.None;
            }
        }

        /// <summary>Gets the root cause of the exception; commonly the most <see cref="Exception.InnerException" /></summary>
        public Exception RootCause
        {
            get
            {
                Exception e = this;
                while (e.InnerException != null)
                    e = e.InnerException;

                return e;
            }
        }

        public Exception GetCause(Type exceptionType)
        {
            if (exceptionType == null)
                throw new ArgumentNullException(nameof(exceptionType));

            if (!typeof(Exception).IsAssignableFrom(exceptionType))
                throw new ArgumentOutOfRangeException(nameof(exceptionType));

            Exception e = this;

            while (e != null)
            {
                if (exceptionType.IsInstanceOfType(e))
                    return e;

                e = e.InnerException;
            }

            return null;
        }

        public T GetCause<T>() where T : Exception
        {
            return (T) GetCause(typeof(T));
        }

        public SvnException GetCause(SvnErrorCode code)
        {
            Exception e = this;

            while (e != null)
            {
                if (e is SvnException svnEx && svnEx.SvnErrorCode == code)
                    return svnEx;

                e = e.InnerException;
            }

            return null;
        }

        public SvnException GetCause(SvnAprErrorCode code)
        {
            return GetCause((SvnErrorCode) (int) code);
        }

        public bool ContainsError(SvnErrorCode code)
        {
            return GetCause(code) != null;
        }

        public bool ContainsError(SvnAprErrorCode code)
        {
            return GetCause(code) != null;
        }

        public bool ContainsError(params SvnErrorCode[] codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            Exception e = this;

            while (e != null)
            {
                if (e is SvnException svnEx)
                    foreach (SvnErrorCode c in codes)
                    {
                        if (svnEx.SvnErrorCode == c)
                            return true;
                    }

                e = e.InnerException;
            }

            return false;
        }

        public unsafe string File
        {
            get
            {
                if (_file == null && _pFile != null)
                {
                    sbyte* pf = _pFile;
                    _pFile = null;
                    try
                    {
                        _file = new string(pf);
                    }
                    catch (AccessViolationException)
                    {
                        /* Subversion will always set file via __FILE__ which comes from
                           a readonly memory segment so this should never crash, but just in case... */
                    }
                }
                return _file;
            }
        }

        public long Line => _line;

        /// <summary>When not NULL, contains a String, Uri or SvnTarget, or an IEnumberable of one of these,
        /// containing (some of) the targets of the command executed. This to help debugging issues from just
        /// handling the exceptions</summary>
        public object Targets
        {
            get => _targets;
            internal set => _targets = value;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            base.GetObjectData(info, context);

            info.AddValue("SvnErrorValue", _errorCode);
            info.AddValue("_file", File);
            info.AddValue("_line", Line);
        }
    }
}
