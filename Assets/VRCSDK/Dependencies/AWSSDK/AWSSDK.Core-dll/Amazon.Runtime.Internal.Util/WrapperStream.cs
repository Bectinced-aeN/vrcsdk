using System;
using System.IO;

namespace Amazon.Runtime.Internal.Util
{
	public class WrapperStream : Stream
	{
		protected Stream BaseStream
		{
			get;
			private set;
		}

		public override bool CanRead => BaseStream.CanRead;

		public override bool CanSeek => BaseStream.CanSeek;

		public override bool CanWrite => BaseStream.CanWrite;

		public override long Length => BaseStream.Length;

		public override long Position
		{
			get
			{
				return BaseStream.Position;
			}
			set
			{
				BaseStream.Position = value;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return BaseStream.ReadTimeout;
			}
			set
			{
				BaseStream.ReadTimeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return BaseStream.WriteTimeout;
			}
			set
			{
				BaseStream.WriteTimeout = value;
			}
		}

		public WrapperStream(Stream baseStream)
		{
			if (baseStream == null)
			{
				throw new ArgumentNullException("baseStream");
			}
			BaseStream = baseStream;
		}

		public Stream GetNonWrapperBaseStream()
		{
			Stream stream = this;
			do
			{
				PartialWrapperStream partialWrapperStream = stream as PartialWrapperStream;
				if (partialWrapperStream != null)
				{
					return partialWrapperStream;
				}
				stream = (stream as WrapperStream).BaseStream;
			}
			while (stream is WrapperStream);
			return stream;
		}

		public Stream GetSeekableBaseStream()
		{
			Stream stream = this;
			do
			{
				if (stream.CanSeek)
				{
					return stream;
				}
				stream = (stream as WrapperStream).BaseStream;
			}
			while (stream is WrapperStream);
			if (!stream.CanSeek)
			{
				throw new InvalidOperationException("Unable to find seekable stream");
			}
			return stream;
		}

		public static Stream GetNonWrapperBaseStream(Stream stream)
		{
			WrapperStream wrapperStream = stream as WrapperStream;
			if (wrapperStream == null)
			{
				return stream;
			}
			return wrapperStream.GetNonWrapperBaseStream();
		}

		public Stream SearchWrappedStream(Func<Stream, bool> condition)
		{
			Stream stream = this;
			do
			{
				if (condition(stream))
				{
					return stream;
				}
				if (!(stream is WrapperStream))
				{
					return null;
				}
				stream = (stream as WrapperStream).BaseStream;
			}
			while (stream != null);
			return stream;
		}

		public static Stream SearchWrappedStream(Stream stream, Func<Stream, bool> condition)
		{
			WrapperStream wrapperStream = stream as WrapperStream;
			if (wrapperStream == null)
			{
				if (!condition(stream))
				{
					return null;
				}
				return stream;
			}
			return wrapperStream.SearchWrappedStream(condition);
		}

		public override void Close()
		{
			BaseStream.Close();
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return BaseStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return BaseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			BaseStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			BaseStream.Write(buffer, offset, count);
		}
	}
}
