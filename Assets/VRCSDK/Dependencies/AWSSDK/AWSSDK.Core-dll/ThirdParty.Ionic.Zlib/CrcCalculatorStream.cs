using System;
using System.IO;

namespace ThirdParty.Ionic.Zlib
{
	public class CrcCalculatorStream : Stream
	{
		private Stream _InnerStream;

		private CRC32 _Crc32;

		private long _length;

		public long TotalBytesSlurped => _Crc32.TotalBytesRead;

		public int Crc32 => _Crc32.Crc32Result;

		public override bool CanRead => _InnerStream.CanRead;

		public override bool CanSeek => _InnerStream.CanSeek;

		public override bool CanWrite => _InnerStream.CanWrite;

		public override long Length
		{
			get
			{
				if (_length == 0L)
				{
					throw new NotImplementedException();
				}
				return _length;
			}
		}

		public override long Position
		{
			get
			{
				return _Crc32.TotalBytesRead;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public CrcCalculatorStream(Stream stream)
		{
			_InnerStream = stream;
			_Crc32 = new CRC32();
		}

		public CrcCalculatorStream(Stream stream, long length)
		{
			_InnerStream = stream;
			_Crc32 = new CRC32();
			_length = length;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int count2 = count;
			if (_length != 0L)
			{
				if (_Crc32.TotalBytesRead >= _length)
				{
					return 0;
				}
				long num = _length - _Crc32.TotalBytesRead;
				if (num < count)
				{
					count2 = (int)num;
				}
			}
			int num2 = _InnerStream.Read(buffer, offset, count2);
			if (num2 > 0)
			{
				_Crc32.SlurpBlock(buffer, offset, num2);
			}
			return num2;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count > 0)
			{
				_Crc32.SlurpBlock(buffer, offset, count);
			}
			_InnerStream.Write(buffer, offset, count);
		}

		public override void Flush()
		{
			_InnerStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}
	}
}
