using System;
using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public class PartialWrapperStream : WrapperStream
	{
		private long initialPosition;

		private long partSize;

		private long RemainingPartSize => partSize - Position;

		public override long Length
		{
			get
			{
				long num = base.Length - initialPosition;
				if (num > partSize)
				{
					num = partSize;
				}
				return num;
			}
		}

		public override long Position
		{
			get
			{
				return base.Position - initialPosition;
			}
			set
			{
				base.Position = initialPosition + value;
			}
		}

		public PartialWrapperStream(Stream stream, long partSize)
			: base(stream)
		{
			if (!stream.CanSeek)
			{
				throw new InvalidOperationException("Base stream of PartialWrapperStream must be seekable");
			}
			initialPosition = stream.Position;
			long num = stream.Length - stream.Position;
			if (partSize == 0L || num < partSize)
			{
				this.partSize = num;
			}
			else
			{
				this.partSize = partSize;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = (int)((count < RemainingPartSize) ? count : RemainingPartSize);
			if (num < 0)
			{
				return 0;
			}
			return base.Read(buffer, offset, num);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num = 0L;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = initialPosition + offset;
				break;
			case SeekOrigin.Current:
				num = base.Position + offset;
				break;
			case SeekOrigin.End:
				num = base.Position + partSize + offset;
				break;
			}
			if (num < initialPosition)
			{
				num = initialPosition;
			}
			else if (num > initialPosition + partSize)
			{
				num = initialPosition + partSize;
			}
			base.Position = num;
			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException();
		}
	}
}
