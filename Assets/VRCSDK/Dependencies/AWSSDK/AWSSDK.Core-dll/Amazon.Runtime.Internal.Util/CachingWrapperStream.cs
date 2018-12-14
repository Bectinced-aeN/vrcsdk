using System;
using System.Collections.Generic;
using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public class CachingWrapperStream : WrapperStream
	{
		private int _cacheLimit;

		private int _cachedBytes;

		public List<byte> AllReadBytes
		{
			get;
			private set;
		}

		public override bool CanSeek => false;

		public override long Position
		{
			get
			{
				throw new NotSupportedException("CachingWrapperStream does not support seeking");
			}
			set
			{
				throw new NotSupportedException("CachingWrapperStream does not support seeking");
			}
		}

		public CachingWrapperStream(Stream baseStream, int cacheLimit)
			: base(baseStream)
		{
			_cacheLimit = cacheLimit;
			AllReadBytes = new List<byte>(cacheLimit);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = base.Read(buffer, offset, count);
			if (_cachedBytes < _cacheLimit)
			{
				int val = _cacheLimit - _cachedBytes;
				int num2 = Math.Min(num, val);
				byte[] array = new byte[num2];
				Array.Copy(buffer, offset, array, 0, num2);
				AllReadBytes.AddRange(array);
				_cachedBytes += num2;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("CachingWrapperStream does not support seeking");
		}
	}
}
