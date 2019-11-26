using System;
using System.IO;
using System.Runtime.InteropServices;
using LibSvnSharp.Interop;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp.Implementation
{
    class SvnStreamWrapper : IDisposable
    {
        readonly AprBaton<SvnStreamWrapper> _streamBaton;
        svn_stream_t _svnStream;
        AprPool _pool;

        internal bool _written;

        public SvnStreamWrapper(Stream stream, bool enableRead, bool enableWrite, AprPool pool)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!enableRead && !enableWrite)
                throw new ArgumentException("enableRead or enableWrite must be set to true");

            Stream = stream;
            _streamBaton = new AprBaton<SvnStreamWrapper>(this);
            _pool = pool;

            if (enableRead && !Stream.CanRead)
                throw new InvalidOperationException("Can't enable reading on an unreadable stream");
            if (enableWrite && !Stream.CanWrite)
                throw new InvalidOperationException("Can't enable writing on an unwritable stream");

            Init(enableRead, enableWrite, stream.CanSeek);
        }

        public void Dispose()
        {
            try
            {
                if (_written)
                    Stream.Flush();
            }
            finally
            {
                _streamBaton?.Dispose();

                _svnStreamRead.Dispose();
                _svnStreamWrite.Dispose();
                _svnStreamMark.Dispose();
                _svnStreamSeek.Dispose();
                _svnStreamClose.Dispose();
            }
        }

        public svn_stream_t Handle
        {
            get
            {
                _pool.Ensure();
                return _svnStream;
            }
        }

        unsafe void Init(bool enableRead, bool enableWrite, bool enableSeek)
        {
            _svnStream = svn_io.svn_stream_create(_streamBaton.Handle, _pool.Handle);

            if (_svnStream == null)
                throw new InvalidOperationException("Can't create svn stream");

            if (enableRead)
                svn_io.svn_stream_set_read(_svnStream, _svnStreamRead.Get());

            if (enableWrite)
                svn_io.svn_stream_set_write(_svnStream, _svnStreamWrite.Get());

            if (enableSeek)
            {
                svn_io.svn_stream_set_mark(_svnStream, _svnStreamMark.Get());
                svn_io.svn_stream_set_seek(_svnStream, _svnStreamSeek.Get());
            }

            svn_io.svn_stream_set_close(_svnStream, _svnStreamClose.Get());
        }

        internal Stream Stream { get; }

        readonly unsafe SafeFuncHandle<svn_read_fn_t> _svnStreamRead =
            new SafeFuncHandle<svn_read_fn_t>(svnStreamRead);

        static unsafe IntPtr svnStreamRead(IntPtr baton, sbyte* buffer, ulong* len)
        {
            // Subversion:
            //                  Handlers are obliged to complete a read or write
            //                  to the maximum extent possible; thus, a short read with no
            //                  associated error implies the end of the input stream, and a short
            //                  write should never occur without an associated error.

            SvnStreamWrapper sw = AprBaton<SvnStreamWrapper>.Get(baton);

            byte[] bytes = new byte[(int)*len];

            int count = sw.Stream.Read(bytes, 0, (int)*len);

            Marshal.Copy(bytes, 0, new IntPtr(buffer), count);

            *len = (ulong)count;

            return IntPtr.Zero;
        }

        readonly unsafe SafeFuncHandle<svn_write_fn_t> _svnStreamWrite =
            new SafeFuncHandle<svn_write_fn_t>(svnStreamWrite);

        static unsafe IntPtr svnStreamWrite(IntPtr baton, sbyte* data, ulong* len)
        {
            if (*len == 0)
                return IntPtr.Zero;

            // Subversion:
            //                  Handlers are obliged to complete a read or write
            //                  to the maximum extent possible; thus, a short read with no
            //                  associated error implies the end of the input stream, and a short
            //                  write should never occur without an associated error.

            SvnStreamWrapper sw = AprBaton<SvnStreamWrapper>.Get(baton);

            byte[] bytes = new byte[(int)*len];

            Marshal.Copy(new IntPtr(data), bytes, 0, bytes.Length);

            sw.Stream.Write(bytes, 0, bytes.Length);
            sw._written = true;

            return IntPtr.Zero;
        }

        readonly SafeFuncHandle<svn_close_fn_t> _svnStreamClose =
            new SafeFuncHandle<svn_close_fn_t>(svnStreamClose);

        static IntPtr svnStreamClose(IntPtr baton)
        {
            SvnStreamWrapper sw = AprBaton<SvnStreamWrapper>.Get(baton);

            if (sw._written)
            {
                sw._written = false;
                sw.Stream.Flush();
            }

            return IntPtr.Zero;
        }

        readonly unsafe SafeFuncHandle<svn_stream_mark_fn_t> _svnStreamMark =
            new SafeFuncHandle<svn_stream_mark_fn_t>(svnStreamMark);

        static unsafe IntPtr svnStreamMark(IntPtr baton, void** mark, IntPtr pool_ptr)
        {
            SvnStreamWrapper sw = AprBaton<SvnStreamWrapper>.Get(baton);

            using (var pool = new AprPool(pool_ptr, false))
            {
                long* pos = (long*)pool.Alloc(sizeof(long));

                *pos = sw.Stream.Position;

                *mark = (void*)pos;
            }

            return IntPtr.Zero;
        }

        readonly unsafe SafeFuncHandle<svn_stream_seek_fn_t> _svnStreamSeek =
            new SafeFuncHandle<svn_stream_seek_fn_t>(svnStreamSeek);

        static unsafe IntPtr svnStreamSeek(IntPtr baton, IntPtr mark)
        {
            SvnStreamWrapper sw = AprBaton<SvnStreamWrapper>.Get(baton);

            long newPos = 0;

            if (mark != IntPtr.Zero)
            {
                newPos = *(long*)mark.ToPointer();
            }

            try
            {
                sw.Stream.Position = newPos;
            }
            catch (Exception ex)
            {
                return SvnException.CreateExceptionSvnError("Seek stream", ex).__Instance;
            }

            return IntPtr.Zero;
        }
    }
}
