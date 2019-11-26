using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Interop
{
    struct Constants
    {
        public const string SVN_CONFIG_CATEGORY_SERVERS = "servers";
        public const string SVN_CONFIG_CATEGORY_CONFIG = "config";

        public const string SVN_CONFIG_SECTION_GLOBAL = "global";
        public const string SVN_CONFIG_SECTION_HELPERS = "helpers";
        public const string SVN_CONFIG_SECTION_MISCELLANY = "miscellany";

        public const string SVN_CONFIG_OPTION_SSL_TRUST_DEFAULT_CA = "ssl-trust-default-ca";
        public const string SVN_CONFIG_OPTION_MEMORY_CACHE_SIZE = "memory-cache-size";
        public const string SVN_CONFIG_OPTION_DIFF_CMD = "diff-cmd";
        public const string SVN_CONFIG_OPTION_DIFF3_CMD = "diff3-cmd";
        public const string SVN_CONFIG_OPTION_MIMETYPES_FILE = "mime-types-file";

        public const long APR_HASH_KEY_STRING = -1;

        public const int APR_SUCCESS = 0;
        public const int APR_OS_START_ERROR = 20000;
        public const int APR_OS_ERRSPACE_SIZE = 50000;
        public const int APR_OS_START_STATUS = (APR_OS_START_ERROR + APR_OS_ERRSPACE_SIZE);
        public const int APR_OS_START_USERERR = (APR_OS_START_STATUS + APR_OS_ERRSPACE_SIZE);
        public const int APR_OS_START_CANONERR = (APR_OS_START_USERERR + (APR_OS_ERRSPACE_SIZE * 10));
        public const int APR_OS_START_EAIERR = (APR_OS_START_CANONERR + APR_OS_ERRSPACE_SIZE);
        public const int APR_OS_START_SYSERR = (APR_OS_START_EAIERR + APR_OS_ERRSPACE_SIZE);

        public const int SVN_ERR_CATEGORY_SIZE = 5000;
        public const int SVN_ERR_BAD_CATEGORY_START = (APR_OS_START_USERERR + (1 * SVN_ERR_CATEGORY_SIZE));
        public const int SVN_ERR_RA_SERF_CATEGORY_START = (APR_OS_START_USERERR + (22 * SVN_ERR_CATEGORY_SIZE));
        public const int SVN_ERR_MALFUNC_CATEGORY_START = (APR_OS_START_USERERR + (23 * SVN_ERR_CATEGORY_SIZE));

        public const long SVN_INVALID_REVNUM = -1;

        public const string SVN_AUTH_PARAM_PREFIX = "svn:auth:";
        public const string SVN_AUTH_PARAM_DEFAULT_USERNAME = SVN_AUTH_PARAM_PREFIX + "username";
        public const string SVN_AUTH_PARAM_DEFAULT_PASSWORD = SVN_AUTH_PARAM_PREFIX + "password";
        public const string SVN_AUTH_PARAM_CONFIG_DIR = SVN_AUTH_PARAM_PREFIX + "config-dir";

        /** All Subversion property names start with this. */
        const string SVN_PROP_PREFIX = "svn:";

        /** The fs revision property that stores a commit's author. */
        public const string SVN_PROP_REVISION_AUTHOR = SVN_PROP_PREFIX + "author";

        /** The fs revision property that stores a commit's log message. */
        public const string SVN_PROP_REVISION_LOG = SVN_PROP_PREFIX + "log";

        /** The fs revision property that stores a commit's date. */
        public const string SVN_PROP_REVISION_DATE = SVN_PROP_PREFIX + "date";

        /** Certificate authority is unknown (i.e. not trusted) */
        public const uint SVN_AUTH_SSL_UNKNOWNCA = unchecked((uint) svn_auth_ssl_enum_t.SVN_AUTH_SSL_UNKNOWNCA);

        /** Properties whose values are interpreted as booleans (such as
         * svn:executable, svn:needs_lock, and svn:special) always fold their
         * value to this.
         */
        public const string SVN_PROP_BOOLEAN_TRUE = "*";

        /** Simple username/password pair credential kind. */
        public const string SVN_AUTH_CRED_SIMPLE = "svn.simple";

        /** Username credential kind. */
        public const string SVN_AUTH_CRED_USERNAME = "svn.username";
    }
}
