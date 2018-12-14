using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DotZLib
{
	public class GZipStream : Stream, IDisposable
	{
		private const string importDLL = "zlibwapi";

		private const int Z_NO_FLUSH = 0;

		private const int Z_PARTIAL_FLUSH = 1;

		private const int Z_SYNC_FLUSH = 2;

		private const int Z_FULL_FLUSH = 3;

		private const int Z_FINISH = 4;

		private const int RSYNCBITS = 12;

		private const int RSYNCMASK = 4095;

		private const int RSYNCHIT = 2047;

		private IntPtr _gzFile;

		private bool _isDisposed;

		private bool _isWriting;

		private bool _isRsyncable;

		private uint rsync_hash = 2047u;

		public override bool CanRead => !_isWriting;

		public override bool CanSeek => false;

		public override bool CanWrite => _isWriting;

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern IntPtr gzopen(string name, string mode);

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl)]
		private static extern int gzbuffer(IntPtr gzFile, uint size);

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl)]
		private static extern int gzclose(IntPtr gzFile);

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl)]
		private static extern int gzwrite(IntPtr gzFile, int data, int length);

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl)]
		private static extern int gzread(IntPtr gzFile, int data, int length);

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl)]
		private static extern int gzgetc(IntPtr gzFile);

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl)]
		private static extern int gzputc(IntPtr gzFile, int c);

		[DllImport("zlibwapi", CallingConvention = CallingConvention.Cdecl)]
		private static extern int gzflush(IntPtr gzFile, int flushMode);

		public GZipStream(string fileName, CompressLevel level, bool rsyncable = false, int bufferSize = -1)
		{
			_isWriting = true;
			_isRsyncable = rsyncable;
			_gzFile = gzopen(fileName, $"wb{(int)level}");
			if (_gzFile == IntPtr.Zero)
			{
				throw new ZLibException(-1, "Could not open " + fileName);
			}
			if (bufferSize > 0)
			{
				int num = gzbuffer(_gzFile, (uint)bufferSize);
				if (num < 0)
				{
					throw new ZLibException(num, "Couldn't set buffer size " + bufferSize + " for filename " + fileName);
				}
			}
		}

		public GZipStream(string fileName)
			: this(fileName, -1)
		{
		}

		public GZipStream(string fileName, int bufferSize)
		{
			_isWriting = false;
			_gzFile = gzopen(fileName, "rb");
			if (_gzFile == IntPtr.Zero)
			{
				throw new ZLibException(-1, "Could not open " + fileName);
			}
			if (bufferSize > 0)
			{
				int num = gzbuffer(_gzFile, (uint)bufferSize);
				if (num < 0)
				{
					throw new ZLibException(num, "Couldn't set buffer size " + bufferSize + " for filename " + fileName);
				}
			}
		}

		public override void Close()
		{
			cleanUp(isDisposing: false);
		}

		~GZipStream()
		{
			cleanUp(isDisposing: false);
		}

		public new void Dispose()
		{
			cleanUp(isDisposing: true);
		}

		private void cleanUp(bool isDisposing)
		{
			if (!_isDisposed)
			{
				gzclose(_gzFile);
				_isDisposed = true;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!CanRead)
			{
				throw new NotSupportedException();
			}
			if (buffer == null)
			{
				throw new ArgumentNullException();
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (offset + count > buffer.Length)
			{
				throw new ArgumentException();
			}
			if (!_isDisposed)
			{
				GCHandle gCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				try
				{
					int num = gzread(_gzFile, gCHandle.AddrOfPinnedObject().ToInt32() + offset, count);
					if (num < 0)
					{
						throw new IOException();
					}
					return num;
				}
				finally
				{
					gCHandle.Free();
				}
			}
			throw new ObjectDisposedException("GZipStream");
		}

		public override int ReadByte()
		{
			if (!CanRead)
			{
				throw new NotSupportedException();
			}
			if (_isDisposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			return gzgetc(_gzFile);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException();
			}
			if (buffer == null)
			{
				throw new ArgumentNullException();
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (offset + count > buffer.Length)
			{
				throw new ArgumentException();
			}
			if (!_isDisposed)
			{
				GCHandle gCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				try
				{
					int num = 0;
					while (true)
					{
						if (num >= count)
						{
							return;
						}
						int num2 = 0;
						if (_isRsyncable)
						{
							while (num + num2 < count)
							{
								rsync_hash = (((rsync_hash << 1) ^ buffer[offset + num + num2]) & 0xFFF);
								num2++;
								if (rsync_hash == 2047)
								{
									break;
								}
							}
						}
						else
						{
							num2 = count;
						}
						if (gzwrite(_gzFile, gCHandle.AddrOfPinnedObject().ToInt32() + offset + num, num2) < 0)
						{
							break;
						}
						if (_isRsyncable)
						{
							Flush(2);
						}
						num += num2;
					}
					throw new IOException();
				}
				finally
				{
					gCHandle.Free();
				}
			}
			throw new ObjectDisposedException("GZipStream");
		}

		public override void WriteByte(byte value)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException();
			}
			if (_isDisposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			if (gzputc(_gzFile, value) < 0)
			{
				throw new IOException();
			}
			if (_isRsyncable)
			{
				rsync_hash = (((rsync_hash << 1) ^ value) & 0xFFF);
				if (rsync_hash == 2047)
				{
					Flush(2);
				}
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
		}

		public void Flush(int flushMode)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException();
			}
			if (flushMode < 0 || flushMode > 4)
			{
				throw new NotSupportedException();
			}
			if (_isDisposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			if (gzflush(_gzFile, flushMode) < 0)
			{
				throw new IOException();
			}
		}
	}
}
