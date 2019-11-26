using System.ComponentModel;

namespace LibSvnSharp
{
    /// <summary>Generated mapping from apr_errno.h</summary>
    public enum SvnAprErrorCode
    {
        APR_SUCCESS = 0,

        /// <summary>System error EPERM</summary>
        [Description("System error EPERM")]
        APR_EPERM = 1,

        APR_ENOENT = 2,

        /// <summary>System error ESRCH</summary>
        [Description("System error ESRCH")]
        APR_ESRCH = 3,

        APR_EINTR = 4,

        /// <summary>System error EIO</summary>
        [Description("System error EIO")]
        APR_EIO = 5,

        /// <summary>System error ENXIO</summary>
        [Description("System error ENXIO")]
        APR_ENXIO = 6,

        /// <summary>System error E2BIG</summary>
        [Description("System error E2BIG")]
        APR_E2BIG = 7,

        /// <summary>System error ENOEXEC</summary>
        [Description("System error ENOEXEC")]
        APR_ENOEXEC = 8,

        APR_EBADF = 9,

        /// <summary>System error ECHILD</summary>
        [Description("System error ECHILD")]
        APR_ECHILD = 10,

        APR_EAGAIN = 11,

        APR_ENOMEM = 12,

        APR_EACCES = 13,

        /// <summary>System error EFAULT</summary>
        [Description("System error EFAULT")]
        APR_EFAULT = 14,

        APR_EEXIST = 17,

        APR_EXDEV = 18,

        /// <summary>System error ENODEV</summary>
        [Description("System error ENODEV")]
        APR_ENODEV = 19,

        APR_ENOTDIR = 20,

        /// <summary>System error EISDIR</summary>
        [Description("System error EISDIR")]
        APR_EISDIR = 21,

        APR_EINVAL = 22,

        APR_ENFILE = 23,

        APR_EMFILE = 24,

        /// <summary>System error ENOTTY</summary>
        [Description("System error ENOTTY")]
        APR_ENOTTY = 25,

        /// <summary>System error EFBIG</summary>
        [Description("System error EFBIG")]
        APR_EFBIG = 27,

        APR_ENOSPC = 28,

        APR_ESPIPE = 29,

        /// <summary>System error EROFS</summary>
        [Description("System error EROFS")]
        APR_EROFS = 30,

        /// <summary>System error EMLINK</summary>
        [Description("System error EMLINK")]
        APR_EMLINK = 31,

        APR_EPIPE = 32,

        /// <summary>System error EDOM</summary>
        [Description("System error EDOM")]
        APR_EDOM = 33,

        APR_ERANGE = 34,

        /// <summary>System error EDEADLK</summary>
        [Description("System error EDEADLK")]
        APR_EDEADLK = 36,

        /// <summary>System error EDEADLOCK</summary>
        [Description("System error EDEADLOCK")]
        APR_EDEADLOCK = 36,

        APR_ENAMETOOLONG = 38,

        /// <summary>System error ENOLCK</summary>
        [Description("System error ENOLCK")]
        APR_ENOLCK = 39,

        /// <summary>System error ENOSYS</summary>
        [Description("System error ENOSYS")]
        APR_ENOSYS = 40,

        APR_ENOTEMPTY = 41,

        [Description("System error EADDRINUSE")]
        APR_EADDRINUSE = 100,

        [Description("System error EADDRNOTAVAIL")]
        APR_EADDRNOTAVAIL = 101,

        APR_EAFNOSUPPORT = 102,

        [Description("System error EALREADY")]
        APR_EALREADY = 103,

        [Description("System error EBADMSG")]
        APR_EBADMSG = 104,

        [Description("System error ECANCELED")]
        APR_ECANCELED = 105,

        APR_ECONNABORTED = 106,

        APR_ECONNREFUSED = 107,

        APR_ECONNRESET = 108,

        [Description("System error EDESTADDRREQ")]
        APR_EDESTADDRREQ = 109,

        APR_EHOSTUNREACH = 110,

        [Description("System error EIDRM")]
        APR_EIDRM = 111,

        APR_EINPROGRESS = 112,

        [Description("System error EISCONN")]
        APR_EISCONN = 113,

        [Description("System error ELOOP")]
        APR_ELOOP = 114,

        [Description("System error EMSGSIZE")]
        APR_EMSGSIZE = 115,

        [Description("System error ENETDOWN")]
        APR_ENETDOWN = 116,

        [Description("System error ENETRESET")]
        APR_ENETRESET = 117,

        APR_ENETUNREACH = 118,

        [Description("System error ENOBUFS")]
        APR_ENOBUFS = 119,

        [Description("System error ENODATA")]
        APR_ENODATA = 120,

        [Description("System error ENOLINK")]
        APR_ENOLINK = 121,

        [Description("System error ENOMSG")]
        APR_ENOMSG = 122,

        [Description("System error ENOPROTOOPT")]
        APR_ENOPROTOOPT = 123,

        [Description("System error ENOSR")]
        APR_ENOSR = 124,

        [Description("System error ENOSTR")]
        APR_ENOSTR = 125,

        [Description("System error ENOTCONN")]
        APR_ENOTCONN = 126,

        [Description("System error ENOTRECOVERABLE")]
        APR_ENOTRECOVERABLE = 127,

        APR_ENOTSOCK = 128,

        [Description("System error ENOTSUP")]
        APR_ENOTSUP = 129,

        APR_EOPNOTSUPP = 130,

        [Description("System error EOTHER")]
        APR_EOTHER = 131,

        [Description("System error EOVERFLOW")]
        APR_EOVERFLOW = 132,

        [Description("System error EOWNERDEAD")]
        APR_EOWNERDEAD = 133,

        [Description("System error EPROTO")]
        APR_EPROTO = 134,

        [Description("System error EPROTONOSUPPORT")]
        APR_EPROTONOSUPPORT = 135,

        [Description("System error EPROTOTYPE")]
        APR_EPROTOTYPE = 136,

        [Description("System error ETIME")]
        APR_ETIME = 137,

        APR_ETIMEDOUT = 138,

        [Description("System error ETXTBSY")]
        APR_ETXTBSY = 139,

        [Description("System error EWOULDBLOCK")]
        APR_EWOULDBLOCK = 140,

        /// <summary>APR_OS_START_ERROR is where the APR specific error values start.</summary>
        [Description("APR_OS_START_ERROR is where the APR specific error values start.")]
        APR_OS_START_ERROR = 20000,

        /// <summary>APR_UTIL_ERRSPACE_SIZE is the size of the space that is reserved for</summary>
        [Description("APR_UTIL_ERRSPACE_SIZE is the size of the space that is reserved for")]
        APR_UTIL_ERRSPACE_SIZE = 20000,

        /// <summary>APR was unable to perform a stat on the file</summary>
        [Description("APR was unable to perform a stat on the file")]
        APR_ENOSTAT = 20001,

        /// <summary>APR was not provided a pool with which to allocate memory</summary>
        [Description("APR was not provided a pool with which to allocate memory")]
        APR_ENOPOOL = 20002,

        /// <summary>APR was given an invalid date</summary>
        [Description("APR was given an invalid date")]
        APR_EBADDATE = 20004,

        /// <summary>APR was given an invalid socket</summary>
        [Description("APR was given an invalid socket")]
        APR_EINVALSOCK = 20005,

        /// <summary>APR was not given a process structure</summary>
        [Description("APR was not given a process structure")]
        APR_ENOPROC = 20006,

        /// <summary>APR was not given a time structure</summary>
        [Description("APR was not given a time structure")]
        APR_ENOTIME = 20007,

        /// <summary>APR was not given a directory structure</summary>
        [Description("APR was not given a directory structure")]
        APR_ENODIR = 20008,

        /// <summary>APR was not given a lock structure</summary>
        [Description("APR was not given a lock structure")]
        APR_ENOLOCK = 20009,

        /// <summary>APR was not given a poll structure</summary>
        [Description("APR was not given a poll structure")]
        APR_ENOPOLL = 20010,

        /// <summary>APR was not given a socket</summary>
        [Description("APR was not given a socket")]
        APR_ENOSOCKET = 20011,

        /// <summary>APR was not given a thread structure</summary>
        [Description("APR was not given a thread structure")]
        APR_ENOTHREAD = 20012,

        /// <summary>APR was not given a thread key structure</summary>
        [Description("APR was not given a thread key structure")]
        APR_ENOTHDKEY = 20013,

        /// <summary>General failure (specific information not available)</summary>
        [Description("General failure (specific information not available)")]
        APR_EGENERAL = 20014,

        /// <summary>There is no more shared memory available</summary>
        [Description("There is no more shared memory available")]
        APR_ENOSHMAVAIL = 20015,

        /// <summary>The specified IP address is invalid</summary>
        [Description("The specified IP address is invalid")]
        APR_EBADIP = 20016,

        /// <summary>The specified netmask is invalid</summary>
        [Description("The specified netmask is invalid")]
        APR_EBADMASK = 20017,

        /// <summary>APR was unable to open the dso object.  For more</summary>
        [Description("APR was unable to open the dso object.  For more")]
        APR_EDSOOPEN = 20019,

        /// <summary>The given path was absolute.</summary>
        [Description("The given path was absolute.")]
        APR_EABSOLUTE = 20020,

        /// <summary>The given path was relative.</summary>
        [Description("The given path was relative.")]
        APR_ERELATIVE = 20021,

        /// <summary>The given path was neither relative nor absolute.</summary>
        [Description("The given path was neither relative nor absolute.")]
        APR_EINCOMPLETE = 20022,

        /// <summary>The given path was above the root path.</summary>
        [Description("The given path was above the root path.")]
        APR_EABOVEROOT = 20023,

        APR_EBADPATH = 20024,

        APR_EPATHWILD = 20025,

        /// <summary>APR_ESYMNOTFOUND Could not find the requested symbol</summary>
        [Description("APR_ESYMNOTFOUND Could not find the requested symbol")]
        APR_ESYMNOTFOUND = 20026,

        /// <summary>The given process wasn't recognized by APR</summary>
        [Description("The given process wasn't recognized by APR")]
        APR_EPROC_UNKNOWN = 20027,

        /// <summary>APR_ENOTENOUGHENTROPY Not enough entropy to continue</summary>
        [Description("APR_ENOTENOUGHENTROPY Not enough entropy to continue")]
        APR_ENOTENOUGHENTROPY = 20028,

        /// <summary>APR_OS_ERRSPACE_SIZE is the maximum number of errors you can fit</summary>
        [Description("APR_OS_ERRSPACE_SIZE is the maximum number of errors you can fit")]
        APR_OS_ERRSPACE_SIZE = 50000,

        /// <summary>APR_OS_START_STATUS is where the APR specific status codes start.</summary>
        [Description("APR_OS_START_STATUS is where the APR specific status codes start.")]
        APR_OS_START_STATUS = 70000,

        /// <summary>Program is currently executing in the child</summary>
        [Description("Program is currently executing in the child")]
        APR_INCHILD = 70001,

        /// <summary>Program is currently executing in the parent</summary>
        [Description("Program is currently executing in the parent")]
        APR_INPARENT = 70002,

        /// <summary>The thread is detached</summary>
        [Description("The thread is detached")]
        APR_DETACH = 70003,

        /// <summary>The thread is not detached</summary>
        [Description("The thread is not detached")]
        APR_NOTDETACH = 70004,

        /// <summary>The child has finished executing</summary>
        [Description("The child has finished executing")]
        APR_CHILD_DONE = 70005,

        /// <summary>The child has not finished executing</summary>
        [Description("The child has not finished executing")]
        APR_CHILD_NOTDONE = 70006,

        /// <summary>The operation did not finish before the timeout</summary>
        [Description("The operation did not finish before the timeout")]
        APR_TIMEUP = 70007,

        /// <summary>The operation was incomplete although some processing</summary>
        [Description("The operation was incomplete although some processing")]
        APR_INCOMPLETE = 70008,

        /// <summary>Getopt found an option not in the option string</summary>
        [Description("Getopt found an option not in the option string")]
        APR_BADCH = 70012,

        /// <summary>Getopt found an option that is missing an argument</summary>
        [Description("Getopt found an option that is missing an argument")]
        APR_BADARG = 70013,

        /// <summary>APR has encountered the end of the file</summary>
        [Description("APR has encountered the end of the file")]
        APR_EOF = 70014,

        /// <summary>APR was unable to find the socket in the poll structure</summary>
        [Description("APR was unable to find the socket in the poll structure")]
        APR_NOTFOUND = 70015,

        /// <summary>APR is using anonymous shared memory</summary>
        [Description("APR is using anonymous shared memory")]
        APR_ANONYMOUS = 70019,

        /// <summary>APR is using a file name as the key to the shared memory</summary>
        [Description("APR is using a file name as the key to the shared memory")]
        APR_FILEBASED = 70020,

        /// <summary>APR is using a shared key as the key to the shared memory</summary>
        [Description("APR is using a shared key as the key to the shared memory")]
        APR_KEYBASED = 70021,

        /// <summary>Ininitalizer value.  If no option has been found, but</summary>
        [Description("Ininitalizer value.  If no option has been found, but")]
        APR_EINIT = 70022,

        /// <summary>The APR function has not been implemented on this</summary>
        [Description("The APR function has not been implemented on this")]
        APR_ENOTIMPL = 70023,

        /// <summary>Two passwords do not match.</summary>
        [Description("Two passwords do not match.")]
        APR_EMISMATCH = 70024,

        /// <summary>The given lock was busy.</summary>
        [Description("The given lock was busy.")]
        APR_EBUSY = 70025,

        /// <summary>APR_UTIL_START_STATUS is where APR-Util starts defining its</summary>
        [Description("APR_UTIL_START_STATUS is where APR-Util starts defining its")]
        APR_UTIL_START_STATUS = 100000,

        /// <summary>APR_OS_START_USEERR is obsolete, defined for compatibility only.</summary>
        [Description("APR_OS_START_USEERR is obsolete, defined for compatibility only.")]
        APR_OS_START_USEERR = 120000,

        /// <summary>APR_OS_START_USERERR are reserved for applications that use APR that</summary>
        [Description("APR_OS_START_USERERR are reserved for applications that use APR that")]
        APR_OS_START_USERERR = 120000,

        SERF_ERROR_START = 120100,

        SERF_ERROR_CLOSING = 120101,

        SERF_ERROR_REQUEST_LOST = 120102,

        SERF_ERROR_WAIT_CONN = 120103,

        SERF_ERROR_DECOMPRESSION_FAILED = 120104,

        SERF_ERROR_BAD_HTTP_RESPONSE = 120105,

        SERF_ERROR_TRUNCATED_HTTP_RESPONSE = 120106,

        SERF_ERROR_SSLTUNNEL_SETUP_FAILED = 120107,

        SERF_ERROR_ABORTED_CONNECTION = 120108,

        SERF_ERROR_CONNECTION_TIMEDOUT = 120112,

        SERF_ERROR_SSL_CERT_FAILED = 120170,

        SERF_ERROR_SSL_COMM_FAILED = 120171,

        SERF_ERROR_AUTHN_FAILED = 120190,

        SERF_ERROR_AUTHN_NOT_SUPPORTED = 120191,

        SERF_ERROR_AUTHN_MISSING_ATTRIBUTE = 120192,

        SERF_ERROR_AUTHN_INITALIZATION_FAILED = 120193,

        SERF_ERROR_ISSUE_IN_TESTSUITE = 120199,

        /// <summary>APR_OS_START_CANONERR is where APR versions of errno values are defined</summary>
        [Description("APR_OS_START_CANONERR is where APR versions of errno values are defined")]
        APR_OS_START_CANONERR = 620000,

        /// <summary>APR_OS_START_EAIERR folds EAI_ error codes from getaddrinfo() into</summary>
        [Description("APR_OS_START_EAIERR folds EAI_ error codes from getaddrinfo() into")]
        APR_OS_START_EAIERR = 670000,

        /// <summary>APR_OS_START_SYSERR folds platform-specific system error values into</summary>
        [Description("APR_OS_START_SYSERR folds platform-specific system error values into")]
        APR_OS_START_SYSERR = 720000,
    }
}
