using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibSvnSharp.Implementation;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public sealed class SvnTools : SvnBase
    {
        const string _hostChars = "._-";
        const string _shareChars = "._-$ ";

        SvnTools() { } // Static class

        /// <summary>
        /// Normalizes the path to a full path
        /// </summary>
        /// <summary>This normalizes drive letters to upper case and hostnames to lowercase to match Subversion 1.6 behavior</summary>
        public static string GetNormalizedFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (path[0] == '\\')
                path = StripLongPrefix(path);

            if (IsNormalizedFullPath(path))
                return path; // Just pass through; no allocations
            if (PathContainsInvalidChars(path) || path.LastIndexOf(':') >= 2)
                throw new ArgumentException(string.Format(SharpSvnStrings.PathXContainsInvalidCharacters, path), nameof(path));

            var retry = true;

            if (path.Length < 260)
            {
                try
                {
                    path = Path.GetFullPath(path);
                    retry = false;
                }
                catch (PathTooLongException) // Path grew by getting full path
                {
                    // Use the retry
                }
                catch (NotSupportedException) // Something fishy is going on
                {
                    // Use the retry
                }
            }

            if (retry)
            {
                path = LongGetFullPath(path);

                if (GetPathRootPart(path) == null)
                    throw new PathTooLongException(string.Format(SharpSvnStrings.PathXTooLongAndNotRooted, path));
            }

            if (path.Length >= 2 && path[1] == ':')
            {
                var c = path[0];

                if ((c >= 'a') && (c <= 'z'))
                    path = char.ToUpperInvariant(c) + path.Substring(1);

                var r = path.TrimEnd('\\');

                if (r.Length > 3)
                    return r;
                return path.Substring(0, 3);
            }

            if (path.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
            {
                var root = GetPathRootPart(path);

                if (root != null && !path.StartsWith(root, StringComparison.Ordinal))
                    path = root + path.Substring(root.Length).TrimEnd('\\');
                else
                    path = path.TrimEnd('\\');
            }
            else
                path = path.TrimEnd('\\');

            return path;
        }

        /// <summary>
        /// Checks whether normalization is required
        /// </summary>
        /// <remarks>This method does not verify the casing of invariant parts</remarks>
        public static bool IsNormalizedFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var c = path.Length;

            if (path.Length < 3)
                return false;

            int i, n;
            if (IsDirSeparator(path, 0))
            {
                if (!IsDirSeparator(path, 1))
                    return false;

                for (i = 2; i < path.Length; i++)
                {
                    var cc = path[i];
                    // Check hostname rules

                    if (!((cc >= 'a' && cc <= 'z') ||
                          (cc >= '0' && cc <= '9') ||
                          0 <= _hostChars.IndexOf(cc)))
                        break;
                }

                if (i == 2 || !IsDirSeparator(path, i))
                    return false;

                i++;

                n = i;

                for (; i < path.Length; i++)
                {
                    // Check share name rules
                    if (!char.IsLetterOrDigit(path, i) && (0 > _shareChars.IndexOf(path[i])))
                        break;
                }

                if (i == n)
                    return false; // "\\server\"
                if (i == c)
                    return true; // "\\server\path"
                if (c > i && !IsDirSeparator(path, i))
                    return false;
                if (c == i + 1)
                    return false; // "\\server\path\"

                i++;
            }
            else if ((path[1] != ':') || !IsDirSeparator(path, 2))
                return false;
            else if (!((path[0] >= 'A') && (path[0] <= 'Z')))
                return false;
            else
                i = 3;

            var invalidMap = InvalidCharMap;

            while (i < c)
            {
                if (i >= c && IsDirSeparator(path, i))
                    return false; // '\'-s behind each other

                if (i < c && path[i] == '.')
                {
                    var j = i;

                    while (j < c && path[j] == '.')
                        j++;

                    if (j < path.Length && IsSeparator(path, j) || j >= c)
                        return false; // Relative path
                }

                n = i;

                while (i < c)
                {
                    ushort cc = path[i];

                    if (cc < invalidMap.Length && 0 != invalidMap[cc])
                        return false;
                    if (cc == '\\' || cc == '/' || cc == ':')
                        break;

                    i++;
                }

                if (n == i)
                    return false;
                if (i == c)
                    return true;
                if (!IsDirSeparator(path, i++))
                    return false;

                if (i == c)
                    return false; // We don't like paths with a '\' at the end
            }

            return true;
        }

        /// <summary>
        /// Checks whether the specified path is an absolute path that doesn't end in an unneeded '\'
        /// </summary>
        public static bool IsAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (path.Length < 3)
                return false;

            int i, n;
            if (IsSeparator(path, 0))
            {
                if (!IsSeparator(path, 1))
                    return false;

                for (i = 2; i < path.Length; i++)
                {
                    if (!char.IsLetterOrDigit(path, i) && 0 > _hostChars.IndexOf(path[i]))
                        break;
                }

                if (i == 2 || i == path.Length || !IsSeparator(path, i))
                    return false;

                i++;

                n = i;

                for (; i < path.Length; i++)
                {
                    if (!char.IsLetterOrDigit(path, i) && 0 > _shareChars.IndexOf(path[i]))
                        break;
                }

                if (i == path.Length)
                    return (i != n);
                if (i == n || !IsSeparator(path, i))
                    return false;

                i++;

                if (i == path.Length)
                    return false; // "\\server\share\"
            }
            else if ((path[1] != ':') || !IsSeparator(path, 2))
                return false;
            else if (!(((path[0] >= 'A') && (path[0] <= 'Z')) || ((path[0] >= 'a') && (path[0] <= 'z'))))
                return false;
            else
                i = 3;

            while (i < path.Length)
            {
                if (IsSeparator(path, i))
                    return false; // '\'-s behind each other

                if (path[i] == '.')
                {
                    int j = i;
                    j++;

                    if (j < path.Length && path[j] == '.')
                        j++;

                    if (j >= path.Length || IsSeparator(path, j))
                        return false; // '\'-s behind each other
                }

                n = i;

                while (i < path.Length && !IsInvalid(path, i) && !IsSeparator(path, i))
                    i++;

                if (n == i)
                    return false;
                if (i == path.Length)
                    return true;
                if (!IsSeparator(path, i++))
                    return false;

                if (i == path.Length)
                    return false; // We don't like paths with a '\' at the end
            }

            return true;
        }

        /// <summary>
        /// Converts a string from a Uri path to a local path name; unescaping when necessary
        /// </summary>
        public static string UriPartToPath(string uriPath)
        {
            if (uriPath == null)
                throw new ArgumentNullException(nameof(uriPath));

            return Uri.UnescapeDataString(uriPath).Replace('/', '\\');
        }

        /// <summary>Gets the filename of the specified target</summary>
        public static string GetFileName(Uri target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target = SvnBase.CanonicalizeUri(target);

            var path = target.AbsolutePath;

            var nEnd = path.LastIndexOf('?');

            if (nEnd < 0)
                nEnd = path.Length - 1;

            if (path.Length != 0 && nEnd <= path.Length && path[nEnd] == '/')
                nEnd--;

            if (nEnd <= 0)
                return "";

            var nStart = path.LastIndexOf('/', nEnd);

            if (nStart >= 0)
                nStart++;
            else
                nStart = 0;

            path = path.Substring(nStart, nEnd - nStart + 1);

            return UriPartToPath(path);
        }

        static string StripLongPrefix(string path)
        {
            if (path.StartsWith("\\\\?\\", StringComparison.Ordinal))
            {
                if (path.StartsWith("\\\\?\\UNC\\", StringComparison.Ordinal))
                    path = '\\' + path.Substring(7);
                else
                    path = path.Substring(4);
            }

            return path;
        }

        static string LongGetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return StripLongPrefix(path);

            throw new NotImplementedException();

            /*
            path = StripLongPrefix(path);

            pin_ptr<const wchar_t> pPath = PtrToStringChars(path);
            wchar_t rPath[1024];
            wchar_t *pPathBuf;

            ZeroMemory(rPath, sizeof(rPath));
            const int sz = (sizeof(rPath) / sizeof(rPath[0]))-1;

            unsigned c = GetFullPathNameW((LPCWSTR)pPath, sz, rPath, nullptr);

            if (c == 0)
                throw gcnew PathTooLongException("GetFullPath for long paths failed");
            else if (c > sz)
            {
                pPathBuf = (wchar_t*)_alloca(sizeof(wchar_t) * (sz + 1));
                c = GetFullPathNameW((LPCWSTR)pPath, sz, pPathBuf, nullptr);
            }
            else
                pPathBuf = rPath;

            path = gcnew String(pPathBuf, 0, c);

            return StripLongPrefix(path);
             */
        }

        static string GetPathRootPart(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (path.Length >= 3 && path[1] == ':' && path[2] == '\\')
            {
                if (path[0] >= 'a' && path[0] <= 'z')
                    return char.ToUpperInvariant(path[0]) + ":\\";
                return path.Substring(0, 3);
            }

            if (!path.StartsWith("\\\\"))
                return null;

            var nSlash = path.IndexOf('\\', 2);

            if (nSlash <= 2)
                return null; // No hostname

            var nEnd = path.IndexOf('\\', nSlash + 1);

            if (nEnd < 0)
                nEnd = path.Length;

            return "\\\\" + path.Substring(2, nSlash - 2).ToLowerInvariant() + path.Substring(nSlash, nEnd - nSlash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsSeparator(string v, int index)
        {
            var c = v[index];

            return (c == '\\') || (c == '/');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsDirSeparator(string v, int index)
        {
            return (v[index] == '\\');
        }

        static bool IsInvalid(string v, int index)
        {
            ushort c = v[index]; // .Net handles index checking
            var invalidChars = InvalidCharMap;

            if (c < invalidChars.Length)
                return (0 != invalidChars[c]);

            return false;
        }
    }
}
