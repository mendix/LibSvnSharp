using System.ComponentModel;

namespace LibSvnSharp
{
    public enum SvnErrorCode
    {
        None = 0,

        [Description("Bad parent pool passed to svn_make_pool()")]
        SVN_ERR_BAD_CONTAINING_POOL = 125000,

        [Description("Bogus filename")]
        SVN_ERR_BAD_FILENAME = 125001,

        [Description("Bogus URL")]
        SVN_ERR_BAD_URL = 125002,

        [Description("Bogus date")]
        SVN_ERR_BAD_DATE = 125003,

        [Description("Bogus mime-type")]
        SVN_ERR_BAD_MIME_TYPE = 125004,

        [Description("Wrong or unexpected property value")]
        SVN_ERR_BAD_PROPERTY_VALUE = 125005,

        [Description("Version file format not correct")]
        SVN_ERR_BAD_VERSION_FILE_FORMAT = 125006,

        [Description("Path is not an immediate child of the specified directory")]
        SVN_ERR_BAD_RELATIVE_PATH = 125007,

        [Description("Bogus UUID")]
        SVN_ERR_BAD_UUID = 125008,

        [Description("Invalid configuration value")]
        SVN_ERR_BAD_CONFIG_VALUE = 125009,

        [Description("Bogus server specification")]
        SVN_ERR_BAD_SERVER_SPECIFICATION = 125010,

        [Description("Unsupported checksum type")]
        SVN_ERR_BAD_CHECKSUM_KIND = 125011,

        [Description("Invalid character in hex checksum")]
        SVN_ERR_BAD_CHECKSUM_PARSE = 125012,

        [Description("Unknown string value of token")]
        SVN_ERR_BAD_TOKEN = 125013,

        [Description("Invalid changelist name")]
        SVN_ERR_BAD_CHANGELIST_NAME = 125014,

        [Description("Invalid atomic")]
        SVN_ERR_BAD_ATOMIC = 125015,

        [Description("Invalid compression method")]
        SVN_ERR_BAD_COMPRESSION_METHOD = 125016,

        [Description("No such XML tag attribute")]
        SVN_ERR_XML_ATTRIB_NOT_FOUND = 130000,

        [Description("<delta-pkg> is missing ancestry")]
        SVN_ERR_XML_MISSING_ANCESTRY = 130001,

        [Description("Unrecognized binary data encoding; can't decode")]
        SVN_ERR_XML_UNKNOWN_ENCODING = 130002,

        [Description("XML data was not well-formed")]
        SVN_ERR_XML_MALFORMED = 130003,

        [Description("Data cannot be safely XML-escaped")]
        SVN_ERR_XML_UNESCAPABLE_DATA = 130004,

        [Description("Unexpected XML element found")]
        SVN_ERR_XML_UNEXPECTED_ELEMENT = 130005,

        [Description("Inconsistent line ending style")]
        SVN_ERR_IO_INCONSISTENT_EOL = 135000,

        [Description("Unrecognized line ending style")]
        SVN_ERR_IO_UNKNOWN_EOL = 135001,

        [Description("Line endings other than expected")]
        SVN_ERR_IO_CORRUPT_EOL = 135002,

        [Description("Ran out of unique names")]
        SVN_ERR_IO_UNIQUE_NAMES_EXHAUSTED = 135003,

        [Description("Framing error in pipe protocol")]
        SVN_ERR_IO_PIPE_FRAME_ERROR = 135004,

        [Description("Read error in pipe")]
        SVN_ERR_IO_PIPE_READ_ERROR = 135005,

        [Description("Write error")]
        SVN_ERR_IO_WRITE_ERROR = 135006,

        [Description("Write error in pipe")]
        SVN_ERR_IO_PIPE_WRITE_ERROR = 135007,

        [Description("Unexpected EOF on stream")]
        SVN_ERR_STREAM_UNEXPECTED_EOF = 140000,

        [Description("Malformed stream data")]
        SVN_ERR_STREAM_MALFORMED_DATA = 140001,

        [Description("Unrecognized stream data")]
        SVN_ERR_STREAM_UNRECOGNIZED_DATA = 140002,

        [Description("Stream doesn't support seeking")]
        SVN_ERR_STREAM_SEEK_NOT_SUPPORTED = 140003,

        [Description("Stream doesn't support this capability")]
        SVN_ERR_STREAM_NOT_SUPPORTED = 140004,

        [Description("Unknown svn_node_kind")]
        SVN_ERR_NODE_UNKNOWN_KIND = 145000,

        [Description("Unexpected node kind found")]
        SVN_ERR_NODE_UNEXPECTED_KIND = 145001,

        [Description("Can't find an entry")]
        SVN_ERR_ENTRY_NOT_FOUND = 150000,

        [Description("Entry already exists")]
        SVN_ERR_ENTRY_EXISTS = 150002,

        [Description("Entry has no revision")]
        SVN_ERR_ENTRY_MISSING_REVISION = 150003,

        [Description("Entry has no URL")]
        SVN_ERR_ENTRY_MISSING_URL = 150004,

        [Description("Entry has an invalid attribute")]
        SVN_ERR_ENTRY_ATTRIBUTE_INVALID = 150005,

        [Description("Can't create an entry for a forbidden name")]
        SVN_ERR_ENTRY_FORBIDDEN = 150006,

        [Description("Obstructed update")]
        SVN_ERR_WC_OBSTRUCTED_UPDATE = 155000,

        [Description("Mismatch popping the WC unwind stack")]
        SVN_ERR_WC_UNWIND_MISMATCH = 155001,

        [Description("Attempt to pop empty WC unwind stack")]
        SVN_ERR_WC_UNWIND_EMPTY = 155002,

        [Description("Attempt to unlock with non-empty unwind stack")]
        SVN_ERR_WC_UNWIND_NOT_EMPTY = 155003,

        [Description("Attempted to lock an already-locked dir")]
        SVN_ERR_WC_LOCKED = 155004,

        [Description("Working copy not locked; this is probably a bug, please report")]
        SVN_ERR_WC_NOT_LOCKED = 155005,

        [Description("Invalid lock")]
        SVN_ERR_WC_INVALID_LOCK = 155006,

        [Description("Path is not a working copy directory")]
        SVN_ERR_WC_NOT_DIRECTORY = 155007,

        [Description("Path is not a working copy directory")]
        SVN_ERR_WC_NOT_WORKING_COPY = 155007,

        [Description("Path is not a working copy file")]
        SVN_ERR_WC_NOT_FILE = 155008,

        [Description("Problem running log")]
        SVN_ERR_WC_BAD_ADM_LOG = 155009,

        [Description("Can't find a working copy path")]
        SVN_ERR_WC_PATH_NOT_FOUND = 155010,

        [Description("Working copy is not up-to-date")]
        SVN_ERR_WC_NOT_UP_TO_DATE = 155011,

        [Description("Left locally modified or unversioned files")]
        SVN_ERR_WC_LEFT_LOCAL_MOD = 155012,

        [Description("Unmergeable scheduling requested on an entry")]
        SVN_ERR_WC_SCHEDULE_CONFLICT = 155013,

        [Description("Found a working copy path")]
        SVN_ERR_WC_PATH_FOUND = 155014,

        [Description("A conflict in the working copy obstructs the current operation")]
        SVN_ERR_WC_FOUND_CONFLICT = 155015,

        [Description("Working copy is corrupt")]
        SVN_ERR_WC_CORRUPT = 155016,

        [Description("Working copy text base is corrupt")]
        SVN_ERR_WC_CORRUPT_TEXT_BASE = 155017,

        [Description("Cannot change node kind")]
        SVN_ERR_WC_NODE_KIND_CHANGE = 155018,

        [Description("Invalid operation on the current working directory")]
        SVN_ERR_WC_INVALID_OP_ON_CWD = 155019,

        [Description("Problem on first log entry in a working copy")]
        SVN_ERR_WC_BAD_ADM_LOG_START = 155020,

        [Description("Unsupported working copy format")]
        SVN_ERR_WC_UNSUPPORTED_FORMAT = 155021,

        [Description("Path syntax not supported in this context")]
        SVN_ERR_WC_BAD_PATH = 155022,

        [Description("Invalid schedule")]
        SVN_ERR_WC_INVALID_SCHEDULE = 155023,

        [Description("Invalid relocation")]
        SVN_ERR_WC_INVALID_RELOCATION = 155024,

        [Description("Invalid switch")]
        SVN_ERR_WC_INVALID_SWITCH = 155025,

        [Description("Changelist doesn't match")]
        SVN_ERR_WC_MISMATCHED_CHANGELIST = 155026,

        [Description("Conflict resolution failed")]
        SVN_ERR_WC_CONFLICT_RESOLVER_FAILURE = 155027,

        [Description("Failed to locate 'copyfrom' path in working copy")]
        SVN_ERR_WC_COPYFROM_PATH_NOT_FOUND = 155028,

        [Description("Moving a path from one changelist to another")]
        SVN_ERR_WC_CHANGELIST_MOVE = 155029,

        [Description("Cannot delete a file external")]
        SVN_ERR_WC_CANNOT_DELETE_FILE_EXTERNAL = 155030,

        [Description("Cannot move a file external")]
        SVN_ERR_WC_CANNOT_MOVE_FILE_EXTERNAL = 155031,

        [Description("Something's amiss with the wc sqlite database")]
        SVN_ERR_WC_DB_ERROR = 155032,

        [Description("The working copy is missing")]
        SVN_ERR_WC_MISSING = 155033,

        [Description("The specified node is not a symlink")]
        SVN_ERR_WC_NOT_SYMLINK = 155034,

        [Description("The specified path has an unexpected status")]
        SVN_ERR_WC_PATH_UNEXPECTED_STATUS = 155035,

        [Description("The working copy needs to be upgraded")]
        SVN_ERR_WC_UPGRADE_REQUIRED = 155036,

        [Description("Previous operation has not finished; run 'cleanup' if it was interrupted")]
        SVN_ERR_WC_CLEANUP_REQUIRED = 155037,

        [Description("The operation cannot be performed with the specified depth")]
        SVN_ERR_WC_INVALID_OPERATION_DEPTH = 155038,

        [Description("Couldn't open a working copy file because access was denied")]
        SVN_ERR_WC_PATH_ACCESS_DENIED = 155039,

        [Description("Mixed-revision working copy was found but not expected")]
        SVN_ERR_WC_MIXED_REVISIONS = 155040,

        [Description("Duplicate targets in svn:externals property")]
        SVN_ERR_WC_DUPLICATE_EXTERNALS_TARGET = 155041,

        [Description("General filesystem error")]
        SVN_ERR_FS_GENERAL = 160000,

        [Description("Error closing filesystem")]
        SVN_ERR_FS_CLEANUP = 160001,

        [Description("Filesystem is already open")]
        SVN_ERR_FS_ALREADY_OPEN = 160002,

        [Description("Filesystem is not open")]
        SVN_ERR_FS_NOT_OPEN = 160003,

        [Description("Filesystem is corrupt")]
        SVN_ERR_FS_CORRUPT = 160004,

        [Description("Invalid filesystem path syntax")]
        SVN_ERR_FS_PATH_SYNTAX = 160005,

        [Description("Invalid filesystem revision number")]
        SVN_ERR_FS_NO_SUCH_REVISION = 160006,

        [Description("Invalid filesystem transaction name")]
        SVN_ERR_FS_NO_SUCH_TRANSACTION = 160007,

        [Description("Filesystem directory has no such entry")]
        SVN_ERR_FS_NO_SUCH_ENTRY = 160008,

        [Description("Filesystem has no such representation")]
        SVN_ERR_FS_NO_SUCH_REPRESENTATION = 160009,

        [Description("Filesystem has no such string")]
        SVN_ERR_FS_NO_SUCH_STRING = 160010,

        [Description("Filesystem has no such copy")]
        SVN_ERR_FS_NO_SUCH_COPY = 160011,

        [Description("The specified transaction is not mutable")]
        SVN_ERR_FS_TRANSACTION_NOT_MUTABLE = 160012,

        [Description("Filesystem has no item")]
        SVN_ERR_FS_NOT_FOUND = 160013,

        [Description("Filesystem has no such node-rev-id")]
        SVN_ERR_FS_ID_NOT_FOUND = 160014,

        [Description("String does not represent a node or node-rev-id")]
        SVN_ERR_FS_NOT_ID = 160015,

        [Description("Name does not refer to a filesystem directory")]
        SVN_ERR_FS_NOT_DIRECTORY = 160016,

        [Description("Name does not refer to a filesystem file")]
        SVN_ERR_FS_NOT_FILE = 160017,

        [Description("Name is not a single path component")]
        SVN_ERR_FS_NOT_SINGLE_PATH_COMPONENT = 160018,

        [Description("Attempt to change immutable filesystem node")]
        SVN_ERR_FS_NOT_MUTABLE = 160019,

        [Description("Item already exists in filesystem")]
        SVN_ERR_FS_ALREADY_EXISTS = 160020,

        [Description("Attempt to remove or recreate fs root dir")]
        SVN_ERR_FS_ROOT_DIR = 160021,

        [Description("Object is not a transaction root")]
        SVN_ERR_FS_NOT_TXN_ROOT = 160022,

        [Description("Object is not a revision root")]
        SVN_ERR_FS_NOT_REVISION_ROOT = 160023,

        [Description("Merge conflict during commit")]
        SVN_ERR_FS_CONFLICT = 160024,

        [Description("A representation vanished or changed between reads")]
        SVN_ERR_FS_REP_CHANGED = 160025,

        [Description("Tried to change an immutable representation")]
        SVN_ERR_FS_REP_NOT_MUTABLE = 160026,

        [Description("Malformed skeleton data")]
        SVN_ERR_FS_MALFORMED_SKEL = 160027,

        [Description("Transaction is out of date")]
        SVN_ERR_FS_TXN_OUT_OF_DATE = 160028,

        [Description("Berkeley DB error")]
        SVN_ERR_FS_BERKELEY_DB = 160029,

        [Description("Berkeley DB deadlock error")]
        SVN_ERR_FS_BERKELEY_DB_DEADLOCK = 160030,

        [Description("Transaction is dead")]
        SVN_ERR_FS_TRANSACTION_DEAD = 160031,

        [Description("Transaction is not dead")]
        SVN_ERR_FS_TRANSACTION_NOT_DEAD = 160032,

        [Description("Unknown FS type")]
        SVN_ERR_FS_UNKNOWN_FS_TYPE = 160033,

        [Description("No user associated with filesystem")]
        SVN_ERR_FS_NO_USER = 160034,

        [Description("Path is already locked")]
        SVN_ERR_FS_PATH_ALREADY_LOCKED = 160035,

        [Description("Path is not locked")]
        SVN_ERR_FS_PATH_NOT_LOCKED = 160036,

        [Description("Lock token is incorrect")]
        SVN_ERR_FS_BAD_LOCK_TOKEN = 160037,

        [Description("No lock token provided")]
        SVN_ERR_FS_NO_LOCK_TOKEN = 160038,

        [Description("Username does not match lock owner")]
        SVN_ERR_FS_LOCK_OWNER_MISMATCH = 160039,

        [Description("Filesystem has no such lock")]
        SVN_ERR_FS_NO_SUCH_LOCK = 160040,

        [Description("Lock has expired")]
        SVN_ERR_FS_LOCK_EXPIRED = 160041,

        [Description("Item is out of date")]
        SVN_ERR_FS_OUT_OF_DATE = 160042,

        [Description("Unsupported FS format")]
        SVN_ERR_FS_UNSUPPORTED_FORMAT = 160043,

        [Description("Representation is being written")]
        SVN_ERR_FS_REP_BEING_WRITTEN = 160044,

        [Description("The generated transaction name is too long")]
        SVN_ERR_FS_TXN_NAME_TOO_LONG = 160045,

        [Description("Filesystem has no such node origin record")]
        SVN_ERR_FS_NO_SUCH_NODE_ORIGIN = 160046,

        [Description("Filesystem upgrade is not supported")]
        SVN_ERR_FS_UNSUPPORTED_UPGRADE = 160047,

        [Description("Filesystem has no such checksum-representation index record")]
        SVN_ERR_FS_NO_SUCH_CHECKSUM_REP = 160048,

        [Description("Property value in filesystem differs from the provided base value")]
        SVN_ERR_FS_PROP_BASEVALUE_MISMATCH = 160049,

        [Description("The filesystem editor completion process was not followed")]
        SVN_ERR_FS_INCORRECT_EDITOR_COMPLETION = 160050,

        [Description("A packed revprop could not be read")]
        SVN_ERR_FS_PACKED_REVPROP_READ_FAILURE = 160051,

        [Description("Could not initialize the revprop caching infrastructure.")]
        SVN_ERR_FS_REVPROP_CACHE_INIT_FAILURE = 160052,

        [Description("Malformed transaction ID string.")]
        SVN_ERR_FS_MALFORMED_TXN_ID = 160053,

        [Description("Corrupt index file.")]
        SVN_ERR_FS_INDEX_CORRUPTION = 160054,

        [Description("Revision not covered by index.")]
        SVN_ERR_FS_INDEX_REVISION = 160055,

        [Description("Item index too large for this revision.")]
        SVN_ERR_FS_INDEX_OVERFLOW = 160056,

        [Description("Container index out of range.")]
        SVN_ERR_FS_CONTAINER_INDEX = 160057,

        [Description("Index files are inconsistent.")]
        SVN_ERR_FS_INDEX_INCONSISTENT = 160058,

        [Description("Lock operation failed")]
        SVN_ERR_FS_LOCK_OPERATION_FAILED = 160059,

        [Description("Unsupported FS type")]
        SVN_ERR_FS_UNSUPPORTED_TYPE = 160060,

        [Description("Container capacity exceeded.")]
        SVN_ERR_FS_CONTAINER_SIZE = 160061,

        [Description("Malformed node revision ID string.")]
        SVN_ERR_FS_MALFORMED_NODEREV_ID = 160062,

        [Description("Invalid generation number data.")]
        SVN_ERR_FS_INVALID_GENERATION = 160063,

        [Description("The repository is locked, perhaps for db recovery")]
        SVN_ERR_REPOS_LOCKED = 165000,

        [Description("A repository hook failed")]
        SVN_ERR_REPOS_HOOK_FAILURE = 165001,

        [Description("Incorrect arguments supplied")]
        SVN_ERR_REPOS_BAD_ARGS = 165002,

        [Description("A report cannot be generated because no data was supplied")]
        SVN_ERR_REPOS_NO_DATA_FOR_REPORT = 165003,

        [Description("Bogus revision report")]
        SVN_ERR_REPOS_BAD_REVISION_REPORT = 165004,

        [Description("Unsupported repository version")]
        SVN_ERR_REPOS_UNSUPPORTED_VERSION = 165005,

        [Description("Disabled repository feature")]
        SVN_ERR_REPOS_DISABLED_FEATURE = 165006,

        [Description("Error running post-commit hook")]
        SVN_ERR_REPOS_POST_COMMIT_HOOK_FAILED = 165007,

        [Description("Error running post-lock hook")]
        SVN_ERR_REPOS_POST_LOCK_HOOK_FAILED = 165008,

        [Description("Error running post-unlock hook")]
        SVN_ERR_REPOS_POST_UNLOCK_HOOK_FAILED = 165009,

        [Description("Repository upgrade is not supported")]
        SVN_ERR_REPOS_UNSUPPORTED_UPGRADE = 165010,

        [Description("Bad URL passed to RA layer")]
        SVN_ERR_RA_ILLEGAL_URL = 170000,

        [Description("Authorization failed")]
        SVN_ERR_RA_NOT_AUTHORIZED = 170001,

        [Description("Unknown authorization method")]
        SVN_ERR_RA_UNKNOWN_AUTH = 170002,

        [Description("Repository access method not implemented")]
        SVN_ERR_RA_NOT_IMPLEMENTED = 170003,

        [Description("Item is out of date")]
        SVN_ERR_RA_OUT_OF_DATE = 170004,

        [Description("Repository has no UUID")]
        SVN_ERR_RA_NO_REPOS_UUID = 170005,

        [Description("Unsupported RA plugin ABI version")]
        SVN_ERR_RA_UNSUPPORTED_ABI_VERSION = 170006,

        [Description("Path is not locked")]
        SVN_ERR_RA_NOT_LOCKED = 170007,

        [Description("Server can only replay from the root of a repository")]
        SVN_ERR_RA_PARTIAL_REPLAY_NOT_SUPPORTED = 170008,

        [Description("Repository UUID does not match expected UUID")]
        SVN_ERR_RA_UUID_MISMATCH = 170009,

        [Description("Repository root URL does not match expected root URL")]
        SVN_ERR_RA_REPOS_ROOT_URL_MISMATCH = 170010,

        [Description("Session URL does not match expected session URL")]
        SVN_ERR_RA_SESSION_URL_MISMATCH = 170011,

        [Description("Can't create tunnel")]
        SVN_ERR_RA_CANNOT_CREATE_TUNNEL = 170012,

        [Description("Can't create session")]
        SVN_ERR_RA_CANNOT_CREATE_SESSION = 170013,

        [Description("RA layer failed to init socket layer")]
        SVN_ERR_RA_DAV_SOCK_INIT = 175000,

        [Description("RA layer failed to create HTTP request")]
        SVN_ERR_RA_DAV_CREATING_REQUEST = 175001,

        [Description("RA layer request failed")]
        SVN_ERR_RA_DAV_REQUEST_FAILED = 175002,

        [Description("RA layer didn't receive requested OPTIONS info")]
        SVN_ERR_RA_DAV_OPTIONS_REQ_FAILED = 175003,

        [Description("RA layer failed to fetch properties")]
        SVN_ERR_RA_DAV_PROPS_NOT_FOUND = 175004,

        [Description("RA layer file already exists")]
        SVN_ERR_RA_DAV_ALREADY_EXISTS = 175005,

        [Description("Invalid configuration value")]
        SVN_ERR_RA_DAV_INVALID_CONFIG_VALUE = 175006,

        [Description("HTTP Path Not Found")]
        SVN_ERR_RA_DAV_PATH_NOT_FOUND = 175007,

        [Description("Failed to execute WebDAV PROPPATCH")]
        SVN_ERR_RA_DAV_PROPPATCH_FAILED = 175008,

        [Description("Malformed network data")]
        SVN_ERR_RA_DAV_MALFORMED_DATA = 175009,

        [Description("Unable to extract data from response header")]
        SVN_ERR_RA_DAV_RESPONSE_HEADER_BADNESS = 175010,

        [Description("Repository has been moved")]
        SVN_ERR_RA_DAV_RELOCATED = 175011,

        [Description("Connection timed out")]
        SVN_ERR_RA_DAV_CONN_TIMEOUT = 175012,

        [Description("URL access forbidden for unknown reason")]
        SVN_ERR_RA_DAV_FORBIDDEN = 175013,

        [Description("The server state conflicts with the requested preconditions")]
        SVN_ERR_RA_DAV_PRECONDITION_FAILED = 175014,

        [Description("The URL doesn't allow the requested method")]
        SVN_ERR_RA_DAV_METHOD_NOT_ALLOWED = 175015,

        [Description("Couldn't find a repository")]
        SVN_ERR_RA_LOCAL_REPOS_NOT_FOUND = 180000,

        [Description("Couldn't open a repository")]
        SVN_ERR_RA_LOCAL_REPOS_OPEN_FAILED = 180001,

        [Description("Svndiff data has invalid header")]
        SVN_ERR_SVNDIFF_INVALID_HEADER = 185000,

        [Description("Svndiff data contains corrupt window")]
        SVN_ERR_SVNDIFF_CORRUPT_WINDOW = 185001,

        [Description("Svndiff data contains backward-sliding source view")]
        SVN_ERR_SVNDIFF_BACKWARD_VIEW = 185002,

        [Description("Svndiff data contains invalid instruction")]
        SVN_ERR_SVNDIFF_INVALID_OPS = 185003,

        [Description("Svndiff data ends unexpectedly")]
        SVN_ERR_SVNDIFF_UNEXPECTED_END = 185004,

        [Description("Svndiff compressed data is invalid")]
        SVN_ERR_SVNDIFF_INVALID_COMPRESSED_DATA = 185005,

        [Description("Apache has no path to an SVN filesystem")]
        SVN_ERR_APMOD_MISSING_PATH_TO_FS = 190000,

        [Description("Apache got a malformed URI")]
        SVN_ERR_APMOD_MALFORMED_URI = 190001,

        [Description("Activity not found")]
        SVN_ERR_APMOD_ACTIVITY_NOT_FOUND = 190002,

        [Description("Baseline incorrect")]
        SVN_ERR_APMOD_BAD_BASELINE = 190003,

        [Description("Input/output error")]
        SVN_ERR_APMOD_CONNECTION_ABORTED = 190004,

        [Description("A path under version control is needed for this operation")]
        SVN_ERR_CLIENT_VERSIONED_PATH_REQUIRED = 195000,

        [Description("Repository access is needed for this operation")]
        SVN_ERR_CLIENT_RA_ACCESS_REQUIRED = 195001,

        [Description("Bogus revision information given")]
        SVN_ERR_CLIENT_BAD_REVISION = 195002,

        [Description("Attempting to commit to a URL more than once")]
        SVN_ERR_CLIENT_DUPLICATE_COMMIT_URL = 195003,

        [Description("Operation does not apply to binary file")]
        SVN_ERR_CLIENT_IS_BINARY_FILE = 195004,

        [Description("Format of an svn:externals property was invalid")]
        SVN_ERR_CLIENT_INVALID_EXTERNALS_DESCRIPTION = 195005,

        [Description("Attempting restricted operation for modified resource")]
        SVN_ERR_CLIENT_MODIFIED = 195006,

        [Description("Operation does not apply to directory")]
        SVN_ERR_CLIENT_IS_DIRECTORY = 195007,

        [Description("Revision range is not allowed")]
        SVN_ERR_CLIENT_REVISION_RANGE = 195008,

        [Description("Inter-repository relocation not allowed")]
        SVN_ERR_CLIENT_INVALID_RELOCATION = 195009,

        [Description("Author name cannot contain a newline")]
        SVN_ERR_CLIENT_REVISION_AUTHOR_CONTAINS_NEWLINE = 195010,

        [Description("Bad property name")]
        SVN_ERR_CLIENT_PROPERTY_NAME = 195011,

        [Description("Two versioned resources are unrelated")]
        SVN_ERR_CLIENT_UNRELATED_RESOURCES = 195012,

        [Description("Path has no lock token")]
        SVN_ERR_CLIENT_MISSING_LOCK_TOKEN = 195013,

        [Description("Operation does not support multiple sources")]
        SVN_ERR_CLIENT_MULTIPLE_SOURCES_DISALLOWED = 195014,

        [Description("No versioned parent directories")]
        SVN_ERR_CLIENT_NO_VERSIONED_PARENT = 195015,

        [Description("Working copy and merge source not ready for reintegration")]
        SVN_ERR_CLIENT_NOT_READY_TO_MERGE = 195016,

        [Description("A file external cannot overwrite an existing versioned item")]
        SVN_ERR_CLIENT_FILE_EXTERNAL_OVERWRITE_VERSIONED = 195017,

        [Description("Invalid path component strip count specified")]
        SVN_ERR_CLIENT_PATCH_BAD_STRIP_COUNT = 195018,

        [Description("Detected a cycle while processing the operation")]
        SVN_ERR_CLIENT_CYCLE_DETECTED = 195019,

        [Description("Working copy and merge source not ready for reintegration")]
        SVN_ERR_CLIENT_MERGE_UPDATE_REQUIRED = 195020,

        [Description("Invalid mergeinfo detected in merge target")]
        SVN_ERR_CLIENT_INVALID_MERGEINFO_NO_MERGETRACKING = 195021,

        [Description("Can't perform this operation without a valid lock token")]
        SVN_ERR_CLIENT_NO_LOCK_TOKEN = 195022,

        [Description("The operation is forbidden by the server")]
        SVN_ERR_CLIENT_FORBIDDEN_BY_SERVER = 195023,

        [Description("A problem occurred; see other errors for details")]
        SVN_ERR_BASE = 200000,

        [Description("Failure loading plugin")]
        SVN_ERR_PLUGIN_LOAD_FAILURE = 200001,

        [Description("Malformed file")]
        SVN_ERR_MALFORMED_FILE = 200002,

        [Description("Incomplete data")]
        SVN_ERR_INCOMPLETE_DATA = 200003,

        [Description("Incorrect parameters given")]
        SVN_ERR_INCORRECT_PARAMS = 200004,

        [Description("Tried a versioning operation on an unversioned resource")]
        SVN_ERR_UNVERSIONED_RESOURCE = 200005,

        [Description("Test failed")]
        SVN_ERR_TEST_FAILED = 200006,

        [Description("Trying to use an unsupported feature")]
        SVN_ERR_UNSUPPORTED_FEATURE = 200007,

        [Description("Unexpected or unknown property kind")]
        SVN_ERR_BAD_PROP_KIND = 200008,

        [Description("Illegal target for the requested operation")]
        SVN_ERR_ILLEGAL_TARGET = 200009,

        [Description("MD5 checksum is missing")]
        SVN_ERR_DELTA_MD5_CHECKSUM_ABSENT = 200010,

        [Description("Directory needs to be empty but is not")]
        SVN_ERR_DIR_NOT_EMPTY = 200011,

        [Description("Error calling external program")]
        SVN_ERR_EXTERNAL_PROGRAM = 200012,

        [Description("Python exception has been set with the error")]
        SVN_ERR_SWIG_PY_EXCEPTION_SET = 200013,

        [Description("A checksum mismatch occurred")]
        SVN_ERR_CHECKSUM_MISMATCH = 200014,

        [Description("The operation was interrupted")]
        SVN_ERR_CANCELLED = 200015,

        [Description("The specified diff option is not supported")]
        SVN_ERR_INVALID_DIFF_OPTION = 200016,

        [Description("Property not found")]
        SVN_ERR_PROPERTY_NOT_FOUND = 200017,

        [Description("No auth file path available")]
        SVN_ERR_NO_AUTH_FILE_PATH = 200018,

        [Description("Incompatible library version")]
        SVN_ERR_VERSION_MISMATCH = 200019,

        [Description("Mergeinfo parse error")]
        SVN_ERR_MERGEINFO_PARSE_ERROR = 200020,

        [Description("Cease invocation of this API")]
        SVN_ERR_CEASE_INVOCATION = 200021,

        [Description("Error parsing revision number")]
        SVN_ERR_REVNUM_PARSE_FAILURE = 200022,

        [Description("Iteration terminated before completion")]
        SVN_ERR_ITER_BREAK = 200023,

        [Description("Unknown changelist")]
        SVN_ERR_UNKNOWN_CHANGELIST = 200024,

        [Description("Reserved directory name in command line arguments")]
        SVN_ERR_RESERVED_FILENAME_SPECIFIED = 200025,

        [Description("Inquiry about unknown capability")]
        SVN_ERR_UNKNOWN_CAPABILITY = 200026,

        [Description("Test skipped")]
        SVN_ERR_TEST_SKIPPED = 200027,

        [Description("APR memcache library not available")]
        SVN_ERR_NO_APR_MEMCACHE = 200028,

        [Description("Couldn't perform atomic initialization")]
        SVN_ERR_ATOMIC_INIT_FAILURE = 200029,

        [Description("SQLite error")]
        SVN_ERR_SQLITE_ERROR = 200030,

        [Description("Attempted to write to readonly SQLite db")]
        SVN_ERR_SQLITE_READONLY = 200031,

        [Description("Unsupported schema found in SQLite db")]
        SVN_ERR_SQLITE_UNSUPPORTED_SCHEMA = 200032,

        [Description("The SQLite db is busy")]
        SVN_ERR_SQLITE_BUSY = 200033,

        [Description("SQLite busy at transaction rollback; resetting all busy SQLite statements to allow rollback")]
        SVN_ERR_SQLITE_RESETTING_FOR_ROLLBACK = 200034,

        [Description("Constraint error in SQLite db")]
        SVN_ERR_SQLITE_CONSTRAINT = 200035,

        [Description("Too many memcached servers configured")]
        SVN_ERR_TOO_MANY_MEMCACHED_SERVERS = 200036,

        [Description("Failed to parse version number string")]
        SVN_ERR_MALFORMED_VERSION_STRING = 200037,

        [Description("Atomic data storage is corrupt")]
        SVN_ERR_CORRUPTED_ATOMIC_STORAGE = 200038,

        [Description("utf8proc library error")]
        SVN_ERR_UTF8PROC_ERROR = 200039,

        [Description("Bad arguments to SQL operators GLOB or LIKE")]
        SVN_ERR_UTF8_GLOB = 200040,

        [Description("Packed data stream is corrupt")]
        SVN_ERR_CORRUPT_PACKED_DATA = 200041,

        [Description("Additional errors:")]
        SVN_ERR_COMPOSED_ERROR = 200042,

        [Description("Parser error: invalid input")]
        SVN_ERR_INVALID_INPUT = 200043,

        [Description("Error parsing arguments")]
        SVN_ERR_CL_ARG_PARSING_ERROR = 205000,

        [Description("Not enough arguments provided")]
        SVN_ERR_CL_INSUFFICIENT_ARGS = 205001,

        [Description("Mutually exclusive arguments specified")]
        SVN_ERR_CL_MUTUALLY_EXCLUSIVE_ARGS = 205002,

        [Description("Attempted command in administrative dir")]
        SVN_ERR_CL_ADM_DIR_RESERVED = 205003,

        [Description("The log message file is under version control")]
        SVN_ERR_CL_LOG_MESSAGE_IS_VERSIONED_FILE = 205004,

        [Description("The log message is a pathname")]
        SVN_ERR_CL_LOG_MESSAGE_IS_PATHNAME = 205005,

        [Description("Committing in directory scheduled for addition")]
        SVN_ERR_CL_COMMIT_IN_ADDED_DIR = 205006,

        [Description("No external editor available")]
        SVN_ERR_CL_NO_EXTERNAL_EDITOR = 205007,

        [Description("Something is wrong with the log message's contents")]
        SVN_ERR_CL_BAD_LOG_MESSAGE = 205008,

        [Description("A log message was given where none was necessary")]
        SVN_ERR_CL_UNNECESSARY_LOG_MESSAGE = 205009,

        [Description("No external merge tool available")]
        SVN_ERR_CL_NO_EXTERNAL_MERGE_TOOL = 205010,

        [Description("Failed processing one or more externals definitions")]
        SVN_ERR_CL_ERROR_PROCESSING_EXTERNALS = 205011,

        [Description("Repository verification failed")]
        SVN_ERR_CL_REPOS_VERIFY_FAILED = 205012,

        [Description("Special code for wrapping server errors to report to client")]
        SVN_ERR_RA_SVN_CMD_ERR = 210000,

        [Description("Unknown svn protocol command")]
        SVN_ERR_RA_SVN_UNKNOWN_CMD = 210001,

        [Description("Network connection closed unexpectedly")]
        SVN_ERR_RA_SVN_CONNECTION_CLOSED = 210002,

        [Description("Network read/write error")]
        SVN_ERR_RA_SVN_IO_ERROR = 210003,

        [Description("Malformed network data")]
        SVN_ERR_RA_SVN_MALFORMED_DATA = 210004,

        [Description("Couldn't find a repository")]
        SVN_ERR_RA_SVN_REPOS_NOT_FOUND = 210005,

        [Description("Client/server version mismatch")]
        SVN_ERR_RA_SVN_BAD_VERSION = 210006,

        [Description("Cannot negotiate authentication mechanism")]
        SVN_ERR_RA_SVN_NO_MECHANISMS = 210007,

        [Description("Editor drive was aborted")]
        SVN_ERR_RA_SVN_EDIT_ABORTED = 210008,

        [Description("Credential data unavailable")]
        SVN_ERR_AUTHN_CREDS_UNAVAILABLE = 215000,

        [Description("No authentication provider available")]
        SVN_ERR_AUTHN_NO_PROVIDER = 215001,

        [Description("All authentication providers exhausted")]
        SVN_ERR_AUTHN_PROVIDERS_EXHAUSTED = 215002,

        [Description("Credentials not saved")]
        SVN_ERR_AUTHN_CREDS_NOT_SAVED = 215003,

        [Description("Authentication failed")]
        SVN_ERR_AUTHN_FAILED = 215004,

        [Description("Read access denied for root of edit")]
        SVN_ERR_AUTHZ_ROOT_UNREADABLE = 220000,

        [Description("Item is not readable")]
        SVN_ERR_AUTHZ_UNREADABLE = 220001,

        [Description("Item is partially readable")]
        SVN_ERR_AUTHZ_PARTIALLY_READABLE = 220002,

        [Description("Invalid authz configuration")]
        SVN_ERR_AUTHZ_INVALID_CONFIG = 220003,

        [Description("Item is not writable")]
        SVN_ERR_AUTHZ_UNWRITABLE = 220004,

        [Description("Diff data source modified unexpectedly")]
        SVN_ERR_DIFF_DATASOURCE_MODIFIED = 225000,

        [Description("Initialization of SSPI library failed")]
        SVN_ERR_RA_SERF_SSPI_INITIALISATION_FAILED = 230000,

        [Description("Server SSL certificate untrusted")]
        SVN_ERR_RA_SERF_SSL_CERT_UNTRUSTED = 230001,

        [Description("Initialization of the GSSAPI context failed")]
        SVN_ERR_RA_SERF_GSSAPI_INITIALISATION_FAILED = 230002,

        [Description("While handling serf response:")]
        SVN_ERR_RA_SERF_WRAPPED_ERROR = 230003,

        [Description("Assertion failure")]
        SVN_ERR_ASSERTION_FAIL = 235000,

        [Description("No non-tracing links found in the error chain")]
        SVN_ERR_ASSERTION_ONLY_TRACING_LINKS = 235001,

        [Description("Unexpected end of ASN1 data")]
        SVN_ERR_ASN1_OUT_OF_DATA = 240000,

        [Description("Unexpected ASN1 tag")]
        SVN_ERR_ASN1_UNEXPECTED_TAG = 240001,

        [Description("Invalid ASN1 length")]
        SVN_ERR_ASN1_INVALID_LENGTH = 240002,

        [Description("ASN1 length mismatch")]
        SVN_ERR_ASN1_LENGTH_MISMATCH = 240003,

        [Description("Invalid ASN1 data")]
        SVN_ERR_ASN1_INVALID_DATA = 240004,

        [Description("Unavailable X509 feature")]
        SVN_ERR_X509_FEATURE_UNAVAILABLE = 240005,

        [Description("Invalid PEM certificate")]
        SVN_ERR_X509_CERT_INVALID_PEM = 240006,

        [Description("Invalid certificate format")]
        SVN_ERR_X509_CERT_INVALID_FORMAT = 240007,

        [Description("Invalid certificate version")]
        SVN_ERR_X509_CERT_INVALID_VERSION = 240008,

        [Description("Invalid certificate serial number")]
        SVN_ERR_X509_CERT_INVALID_SERIAL = 240009,

        [Description("Found invalid algorithm in certificate")]
        SVN_ERR_X509_CERT_INVALID_ALG = 240010,

        [Description("Found invalid name in certificate")]
        SVN_ERR_X509_CERT_INVALID_NAME = 240011,

        [Description("Found invalid date in certificate")]
        SVN_ERR_X509_CERT_INVALID_DATE = 240012,

        [Description("Found invalid public key in certificate")]
        SVN_ERR_X509_CERT_INVALID_PUBKEY = 240013,

        [Description("Found invalid signature in certificate")]
        SVN_ERR_X509_CERT_INVALID_SIGNATURE = 240014,

        [Description("Found invalid extensions in certificate")]
        SVN_ERR_X509_CERT_INVALID_EXTENSIONS = 240015,

        [Description("Unknown certificate version")]
        SVN_ERR_X509_CERT_UNKNOWN_VERSION = 240016,

        [Description("Certificate uses unknown public key algorithm")]
        SVN_ERR_X509_CERT_UNKNOWN_PK_ALG = 240017,

        [Description("Certificate signature mismatch")]
        SVN_ERR_X509_CERT_SIG_MISMATCH = 240018,

        [Description("Certficate verification failed")]
        SVN_ERR_X509_CERT_VERIFY_FAILED = 240019,
    }
}
