using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Apr;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp.Implementation
{
    /// <summary>Main class of Subversion api. This class is responsible for loading the unmanaged api</summary>
    public class SvnBase : MarshalByRefObject
    {
        internal static string _clientName;
        internal static readonly List<string> _clientNames = new List<string>();

        static volatile int _ensurer = 0;
        static readonly object _ensurerLock = new object();

        static bool _aprInitialized;
        static string _admDir;

        static SvnBase()
        {
            var name = new AssemblyName(typeof(SvnBase).Assembly.FullName);

            var platform = Environment.OSVersion.Platform.ToString();

            // This part is appended to something like "SVN/1.5.0 (LibSvnSharp:branch/1.5.X@12345) WIN32/" to form the user agent on web requests

            _clientName = $"{Environment.OSVersion.Version.ToString(2)} {name.Name}/{name.Version}";

            _clientNames.Add(platform);
            _clientNames.Add(name.Name);

            EnsureLoaded();
        }

        internal static void EnsureLoaded()
        {
            if (Interlocked.CompareExchange(ref _ensurer, 1, 0) < 2)
            {
                lock (_ensurerLock)
                {
                    if (!_aprInitialized)
                    {
                        _aprInitialized = true;

                        if (apr_general.apr_initialize() != 0) // First
                            throw new InvalidOperationException();

                        var error = svn_dso.svn_dso_initialize2(); // Before first pool
                        if (error != null)
                            throw SvnException.Create(error);

                        var pool = svn_pools.svn_pool_create(null);

                        var allocator = apr_pools.apr_pool_allocator_get(pool);

                        if (allocator != null)
                            apr_allocator.apr_allocator_max_free_set(allocator, 1); // Keep a maximum of 1 free block

                        svn_utf.svn_utf_initialize2(true, pool);
                        {
                            var settings = svn_cache_config.svn_cache_config_get();
                            settings.cache_size = 0;
                            settings.file_handle_count = 0;
                            settings.single_threaded = false;
                            svn_cache_config.svn_cache_config_set(settings);
                        }

                        if (Environment.GetEnvironmentVariable("SVN_ASP_DOT_NET_HACK") != null)
                        {
                            svn_wc.svn_wc_set_adm_dir("_svn", pool);
                        }

                        error = svn_ra.svn_ra_initialize(pool);
                        if (error != null)
                            throw SvnException.Create(error);

                        _admDir = svn_wc.svn_wc_get_adm_dir(pool);

                        InstallAbortHandler();
                        //InstallSslDialogHandler();

                        //int r = libssh2_init(0);
                        //if (r != 0)
                        //    throw new InvalidOperationException("Can't initialize libssh2");

                        var v = Interlocked.Exchange(ref _ensurer, 2);

                        System.Diagnostics.Debug.Assert(v == 1);
                    }
                }
            }
        }

        static readonly unsafe SafeFuncHandle<svn_error_malfunction_handler_t> libsvnsharp_malfunction_handler =
            new SafeFuncHandle<svn_error_malfunction_handler_t>(_libsvnsharp_malfunction_handler);

        static unsafe IntPtr _libsvnsharp_malfunction_handler(int canReturn, sbyte* file, int line, sbyte* expr)
        {
            throw new SvnMalfunctionException(Utf8_PtrToString(expr), Utf8_PtrToString(file), line);
        }

        internal static void InstallAbortHandler()
        {
            svn_error.svn_error_set_malfunction_handler(libsvnsharp_malfunction_handler.Get());
        }

        internal SvnBase() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T[] NewSingleItemArray<T>(T value)
        {
            var items = new T[1];
            items[0] = value;

            return items;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ICollection<T> NewSingleItemCollection<T>(T value)
        {
            return NewSingleItemArray(value);
        }

        [ThreadStatic] static AprPool _threadPool;

        /// <summary>Gets a small thread-local pool usable for small one shot actions</summary>
        /// <remarks>The memory allocated by the pool is only freed after the thread is closed; so use with care</remarks>
        internal static AprPool SmallThreadPool
        {
            get
            {
                if (_threadPool == null || !_threadPool.IsValid()) // Recreate if disposed for some reason
                    _threadPool = new AprPool();

                return _threadPool;
            }
        }

        internal static unsafe string Utf8_PtrToString(sbyte* ptr)
        {
            if (ptr == null)
                return null;

            if (*ptr == 0)
                return "";

            return Utf8_PtrToString(ptr, strlen((byte*) ptr));
        }

        internal static unsafe string Utf8_PtrToString(sbyte* ptr, int length)
        {
            if (ptr == null || length < 0)
                return null;

            if (*ptr == 0)
                return "";

            return new string(ptr, 0, length, Encoding.UTF8);
        }

        internal static unsafe string Utf8_PathPtrToString(sbyte* ptr, AprPool pool)
        {
            if (ptr == null || pool == null)
                return null;

            if (*ptr == 0)
                return string.Empty;

            return Utf8_PtrToString(svn_dirent_uri.svn_dirent_local_style(ptr, pool.Handle));
        }

        internal static unsafe Uri Utf8_PtrToUri(sbyte* ptr, SvnNodeKind nodeKind)
        {
            if (ptr == null)
                return null;

            var url = Utf8_PtrToString(ptr);

            if (url == null)
                return null;

            if (nodeKind == SvnNodeKind.Directory && !url.EndsWith("/", StringComparison.Ordinal))
                url += "/";

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return uri;

            return null;
        }

        internal static unsafe byte[] PtrToByteArray(sbyte* ptr, int length)
        {
            if (ptr == null || length < 0)
                return null;

            if (length > 0)
            {
                var span = new ReadOnlySpan<byte>(ptr, length);
                return span.ToArray();
            }

            return Array.Empty<byte>();
        }

        internal static unsafe object PtrToStringOrByteArray(sbyte* ptr, int length)
        {
            if (ptr == null || length < 0)
                return null;
            if (length == 0)
                return "";

            for (int i = 0; i < length; i++)
            {
                if (ptr[i] == 0)
                {
                    // A string that contains a 0 byte can never be valid Utf-8
                    return PtrToByteArray(ptr, length);
                }
            }

            try
            {
                return Utf8_PtrToString(ptr, length);
            }
            catch (DecoderFallbackException)
            {
                return PtrToByteArray(ptr, length);
            }
            catch (ArgumentException)
            {
                return PtrToByteArray(ptr, length);
            }
        }

        internal static DateTime DateTimeFromAprTime(long aprTime)
        {
            if (aprTime == 0)
                return DateTime.MinValue;

            var aprTimeBase = new DateTime(1970, 1, 1).Ticks;
            return new DateTime(aprTime * 10 + aprTimeBase, DateTimeKind.Utc);
        }

        internal static long AprTimeFromDateTime(DateTime time)
        {
            long aprTimeBase = new DateTime(1970, 1, 1).Ticks;

            if (time != DateTime.MinValue)
                return (time.ToUniversalTime().Ticks - aprTimeBase) / 10L;

            return 0;
        }

        /// <summary>Gets a boolean indicating whether the path is a file path (and not a Uri)</summary>
        internal static bool IsNotUri(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Use the same stupid algorithm subversion uses to choose between Uri's and paths
            for (int i = 0; i < path.Length; i++)
            {
                var c = path[i];
                switch (c)
                {
                    case '\\':
                    case '/':
                        return true;
                    case ':':
                        if (i < path.Length - 2)
                        {
                            if ((path[i + 1] == '/') && (path[i + 2] == '/'))
                                return false;
                        }
                        return true;
                    case '+':
                    case '-':
                    case '_':
                        break;
                    default:
                        if (!char.IsLetter(c))
                            return true;
                        break;
                }
            }
            return true;
        }

        internal static bool IsValidReposUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!uri.IsAbsoluteUri)
                return false;

            if (string.IsNullOrEmpty(uri.Scheme))
                return false;

            return true;
        }

        internal static Uri CanonicalizeUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(SharpSvnStrings.UriIsNotAbsolute, nameof(uri));

            var path = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

            var schemeOk = !ContainsUpper(uri.Scheme) && !ContainsUpper(uri.Host);

            if (schemeOk && (path.Length == 0 || (path[path.Length - 1] != '/' && path.IndexOf('\\') < 0) && !path.Contains("//"))
                         && !(uri.IsFile && (uri.IsUnc ? string.Equals("localhost", uri.Host) : char.IsLower(uri.LocalPath, 0))))
                return uri;

            var components = uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.SafeUnescaped);
            if (!schemeOk)
            {
                var nStart = components.IndexOf(':');

                if (nStart > 0)
                {
                    // Subversion 1.6 will require scheme and hostname in lowercase
                    for (var i = 0; i < nStart; i++)
                        if (!char.IsLower(components, i))
                        {
                            components = components.Substring(0, nStart).ToLowerInvariant() + components.Substring(nStart);
                            break;
                        }

                    var nAt = components.IndexOf('@', nStart);

                    if (nAt < 0)
                        nAt = nStart + 2;
                    else
                        nAt++;

                    for (var i = nAt; i < components.Length; i++)
                        if (!char.IsLower(components, i))
                        {
                            components = components.Substring(0, nAt) + components.Substring(nAt) + 1;
                            break;
                        }
                }
            }

            // Create a new uri with all / and \ characters at the end removed

            if (!Uri.TryCreate(components, UriKind.Absolute, out var root))
                throw new ArgumentException("Invalid Uri value in scheme or server", nameof(uri));

            var part = RemoveDoubleSlashes("/" + path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            if (root.IsFile)
            {
                if (!root.IsUnc || root.Host == "localhost")
                {
                    if (part.Length >= 3 && part[0] == '/' && part[2] == ':' && part[1] >= 'a' && part[1] <= 'z')
                    {
                        part = "/" + char.ToUpperInvariant(part[1]) + part.Substring(2);

                        if (part.Length == 3)
                            part += "/";
                    }

                    if (root.IsUnc)
                        part = "//localhost/" + part.TrimStart('/');
                }
                else
                {
                    part = part.TrimStart('/');
                }
            }

            if (!Uri.TryCreate(part, UriKind.Relative, out var suffix))
                throw new ArgumentException("Invalid Uri value in path", nameof(uri));

            if (Uri.TryCreate(root, suffix, out var result))
                return result;

            return uri;
        }

        internal static Uri PathToUri(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var sb = new StringBuilder();

            var afterFirst = false;

            foreach (var p in path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            {
                if (afterFirst)
                    sb.Append('/');
                else
                    afterFirst = true;

                sb.Append(Uri.EscapeDataString(p));
            }

            if (Uri.TryCreate(sb.ToString(), UriKind.Relative, out var result))
                return result;

            throw new ArgumentException("Path is not convertible to uri part", nameof(path));
        }

        internal static string RemoveDoubleSlashes(string input)
        {
            int n;

            while (0 <= (n = input.IndexOf("//", StringComparison.Ordinal)))
                input = input.Remove(n, 1);

            return input;
        }

        static bool ContainsUpper(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            for (var i = 0; i < value.Length; i++)
                if (char.IsUpper(value, i))
                    return true;

            return false;
        }

        internal static apr_array_header_t AllocArray(ICollection<string> strings, AprPool pool)
        {
            if (strings == null)
                throw new ArgumentNullException(nameof(strings));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));
            if (strings.Any(s => s == null))
                throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(strings));

            var aprStrings = new AprArray<string, AprCStrMarshaller>(strings, pool);
            return aprStrings.Handle;
        }

        internal static apr_array_header_t AllocDirentArray(ICollection<string> paths, AprPool pool)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));
            if (paths.Any(s => s == null))
                throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(paths));

            var aprPaths = new AprArray<string, AprCStrDirentMarshaller>(paths, pool);
            return aprPaths.Handle;
        }

        internal static apr_array_header_t AllocCopyArray<TSvnTarget>(ICollection<TSvnTarget> targets, AprPool pool)
            where TSvnTarget : SvnTarget
        {
            if (targets == null)
                throw new ArgumentNullException(nameof(targets));

            foreach (SvnTarget s in targets)
            {
                if (s == null)
                    throw new ArgumentException(SharpSvnStrings.ItemInListIsNull, nameof(targets));
            }

            var aprTargets = new AprArray<SvnTarget, SvnCopyTargetMarshaller>(targets, pool);
            return aprTargets.Handle;
        }

        internal static T[] ExtendArray<T>(T[] @from, T value)
        {
            var to = new T[@from?.Length + 1 ?? 1];

            @from?.CopyTo(to, 0);

            to[to.Length - 1] = value;

            return to;
        }

        internal static apr_array_header_t CreateChangeListsList(ICollection<string> changelists, AprPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (changelists != null && changelists.Count > 0)
                return AllocArray(changelists, pool);

            return null;
        }

        internal static unsafe apr_hash_t CreateRevPropList(SvnRevisionPropertyCollection revProps, AprPool pool)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            if (revProps != null && revProps.Count != 0)
            {
                apr_hash_t items = apr_hash.apr_hash_make(pool.Handle);

                foreach (SvnPropertyValue value in revProps)
                {
                    sbyte* key = pool.AllocString(value.Key);

                    var val = pool.AllocSvnString((byte[])value.RawValue);

                    apr_hash.apr_hash_set(items, new IntPtr(key), Constants.APR_HASH_KEY_STRING, val.__Instance);
                }

                return items;
            }

            return null;
        }

        internal static unsafe SvnPropertyCollection CreatePropertyDictionary(apr_hash_t propHash, AprPool pool)
        {
            if (propHash == null)
                throw new ArgumentNullException(nameof(propHash));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var _properties = new SvnPropertyCollection();

            for (var hi = apr_hash.apr_hash_first(pool.Handle, propHash); hi != null; hi = apr_hash.apr_hash_next(hi))
            {
                sbyte* pKey;
                long keyLen = 0;
                svn_string_t.__Internal* propValPtr;

                apr_hash.apr_hash_this(hi, (void**)&pKey, ref keyLen, (void**)&propValPtr);

                var propVal = svn_string_t.__CreateInstance(new IntPtr(propValPtr));
                _properties.Add(SvnPropertyValue.Create(pKey, propVal, null));
            }

            return _properties;
        }

        internal static string UriToString(Uri value)
        {
            if (value == null)
                return null;

            if (value.IsAbsoluteUri)
                return value.GetComponents(
                    UriComponents.SchemeAndServer |
                    UriComponents.UserInfo |
                    UriComponents.Path, UriFormat.UriEscaped);

            return Uri.EscapeUriString(value.ToString()); // Escape back to valid uri form
        }

        internal static string UriToCanonicalString(Uri value)
        {
            if (value == null)
                return null;

            var name = UriToString(CanonicalizeUri(value));

            if (!string.IsNullOrEmpty(name) && (name[name.Length - 1] == '/'))
                return name.TrimEnd('/'); // "svn://host:port" is canoncialized to "svn://host:port/" by the .Net Uri class

            return name;
        }

        internal static bool PathContainsInvalidChars(string path)
        {
            var invalidChars = InvalidCharMap;

            foreach (var c in path)
            {
                ushort cs = c;

                if (cs < invalidChars.Length
                    && invalidChars[cs] != 0)
                {
                    return false; //TODO: should be true ?!
                }
            }

            return false;
        }

        static char[] _invalidCharMap;

        internal static char[] InvalidCharMap
        {
            get
            {
                if (_invalidCharMap == null)
                    GenerateInvalidCharMap();

                return _invalidCharMap;
            }
        }

        internal static void GenerateInvalidCharMap()
        {
            var invalid = new List<char>(128); // Typical required: 124

            foreach (var c in Path.GetInvalidPathChars())
            {
                ushort cs = c;

                while (cs >= invalid.Count)
                    invalid.Add((char) 0);

                invalid[cs] = (char) 1;
            }

            _invalidCharMap = invalid.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int strlen(byte* ptr)
        {
            // IndexOf processes memory in aligned chunks, and thus it won't crash even if it accesses memory beyond the null terminator.
            var span = new ReadOnlySpan<byte>(ptr, int.MaxValue);
            int length = span.IndexOf((byte)'\0');
            if (length < 0)
                throw new InvalidOperationException("The string must be null-terminated.");

            return length;
        }
    }
}
