namespace LibSvnSharp
{
    public sealed class SvnConfigNames
    {
        public const string ServersCategory = "servers";
        /*
        public const string     GroupsSection                       = SVN_CONFIG_SECTION_GROUPS;
        public const string     GlobalSection                       = SVN_CONFIG_SECTION_GLOBAL;
        public const string     HttpProxyHost                       = SVN_CONFIG_OPTION_HTTP_PROXY_HOST;
        public const string     HttpProxyPort                       = SVN_CONFIG_OPTION_HTTP_PROXY_PORT;
        public const string     HttpProxyUserName                   = SVN_CONFIG_OPTION_HTTP_PROXY_USERNAME;
        public const string     HttpProxyPassword                   = SVN_CONFIG_OPTION_HTTP_PROXY_PASSWORD;
        public const string     HttpProxyExceptions                 = SVN_CONFIG_OPTION_HTTP_PROXY_EXCEPTIONS;
        public const string     HttpTimeout                         = SVN_CONFIG_OPTION_HTTP_TIMEOUT;
        public const string     HttpCompression                     = SVN_CONFIG_OPTION_HTTP_COMPRESSION;
        /* SVN_CONFIG_OPTION_NEON_DEBUG_MASK: only available in debug builds of svn itself * /
        public const string     HttpAuthTypes                       = SVN_CONFIG_OPTION_HTTP_AUTH_TYPES;
        public const string     HttpLibrary                         = SVN_CONFIG_OPTION_HTTP_LIBRARY;

        public const string     HttpBulkUpdates                     = SVN_CONFIG_OPTION_HTTP_BULK_UPDATES;
        public const string     HttpMaxConnections                  = SVN_CONFIG_OPTION_HTTP_MAX_CONNECTIONS;
        public const string     HttpChunkedRequests                 = SVN_CONFIG_OPTION_HTTP_CHUNKED_REQUESTS;
        /* SVN_CONFIG_OPTION_SERF_LOG_COMPONENTS * /
        /* SVN_CONFIG_OPTION_SERF_LOG_LEVEL * /

        public const string     SslAuthorityFiles                   = SVN_CONFIG_OPTION_SSL_AUTHORITY_FILES;
        public const string     SslTrustDefaultCertificateAuthority = SVN_CONFIG_OPTION_SSL_TRUST_DEFAULT_CA;
        public const string     SslClientCertificateFile            = SVN_CONFIG_OPTION_SSL_CLIENT_CERT_FILE;
        public const string     SslClientCertificatePassword        = SVN_CONFIG_OPTION_SSL_CLIENT_CERT_PASSWORD;
        */

        public const string ConfigCategory                          = "config";
        /*
        public const string     AuthSection                         = SVN_CONFIG_SECTION_AUTH;
        public const string         StorePasswords                  = SVN_CONFIG_OPTION_STORE_PASSWORDS;
        public const string         StoreAuthCredentials            = SVN_CONFIG_OPTION_STORE_AUTH_CREDS;
        public const string         PasswordStores                  = SVN_CONFIG_OPTION_PASSWORD_STORES;
        public const string         ClientCertFilePrompt            = SVN_CONFIG_OPTION_SSL_CLIENT_CERT_FILE_PROMPT;

        public const string     HelpersSection                      = SVN_CONFIG_SECTION_HELPERS;
        public const string         EditorCommand                   = SVN_CONFIG_OPTION_EDITOR_CMD;
        public const string         DiffExtensions                  = SVN_CONFIG_OPTION_DIFF_EXTENSIONS;
        public const string         DiffCommand                     = SVN_CONFIG_OPTION_DIFF_CMD;
        public const string         Diff3Command                    = SVN_CONFIG_OPTION_DIFF3_CMD;
        public const string         Diff3HasProgramArguments        = SVN_CONFIG_OPTION_DIFF3_HAS_PROGRAM_ARG;
        public const string         MergeToolCommand                = SVN_CONFIG_OPTION_MERGE_TOOL_CMD;

        public const string     MiscellanySection                   = SVN_CONFIG_SECTION_MISCELLANY;
        public const string         GlobalIgnores                   = SVN_CONFIG_OPTION_GLOBAL_IGNORES;
        public const string         LogEncoding                     = SVN_CONFIG_OPTION_LOG_ENCODING;
        public const string         UseCommitTimes                  = SVN_CONFIG_OPTION_USE_COMMIT_TIMES;
        public const string         EnableAutoProps                 = SVN_CONFIG_OPTION_ENABLE_AUTO_PROPS;
        public const string         NoUnlock                        = SVN_CONFIG_OPTION_NO_UNLOCK;
        public const string         MimeTypesFile                   = SVN_CONFIG_OPTION_MIMETYPES_FILE;
        public const string         PreservedConflictFileExtensions = SVN_CONFIG_OPTION_PRESERVED_CF_EXTS;
        public const string         InteractiveConflicts            = SVN_CONFIG_OPTION_INTERACTIVE_CONFLICTS;
        public const string         MemoryCacheSize                 = SVN_CONFIG_OPTION_MEMORY_CACHE_SIZE;
        public const string         DiffIgnoreContentType           = SVN_CONFIG_OPTION_DIFF_IGNORE_CONTENT_TYPE;

        public const string     TunnelsSection                      = SVN_CONFIG_SECTION_TUNNELS;

        public const string     AutoPropsSection                    = SVN_CONFIG_SECTION_AUTO_PROPS;

        public const string     WorkingCopySection                  = SVN_CONFIG_SECTION_WORKING_COPY;
        public const string         SqliteExclusive                 = SVN_CONFIG_OPTION_SQLITE_EXCLUSIVE;
        public const string         SqliteExclusiveClients          = SVN_CONFIG_OPTION_SQLITE_EXCLUSIVE_CLIENTS;
        public const string         SqliteBusyTimeout               = SVN_CONFIG_OPTION_SQLITE_BUSY_TIMEOUT;

        public static readonly ReadOnlyCollection<String> SvnDefaultGlobalIgnores
            = new ReadOnlyCollection<String>(
                SVN_CONFIG_DEFAULT_GLOBAL_IGNORES.Split(' '));
        */
    }
}
