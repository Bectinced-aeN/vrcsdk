using System;
using System.IO;

namespace librsync.net
{
	internal class SignatureStream : Stream
	{
		private const int BlocksToBuffer = 100;

		private const long HeaderLength = 12L;

		private Stream inputStream;

		private BinaryReader inputReader;

		private SignatureJobSettings settings;

		private MemoryStream bufferStream;

		private long currentPosition;

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => false;

		public override long Length
		{
			get
			{
				long num = (inputStream.Length + settings.BlockLength - 1) / settings.BlockLength;
				return 12 + num * (4 + settings.StrongSumLength);
			}
		}

		public override long Position
		{
			get
			{
				return currentPosition;
			}
			set
			{
				Seek(value, SeekOrigin.Begin);
			}
		}

		public SignatureStream(Stream inputStream, SignatureJobSettings settings)
		{
			this.inputStream = inputStream;
			inputReader = new BinaryReader(inputStream);
			this.settings = settings;
			InitializeHeader();
			currentPosition = 0L;
		}

		private int ReadInternalSync(byte[] buffer, int offset, int count)
		{
			if (bufferStream.Position == bufferStream.Length)
			{
				FillBuffer();
			}
			int num = bufferStream.Read(buffer, offset, count);
			currentPosition += num;
			return num;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ReadInternalSync(buffer, offset, count);
		}

		private void FillBuffer()
		{
			bufferStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(bufferStream);
			for (int i = 0; i < 100; i++)
			{
				byte[] array = inputReader.ReadBytes(settings.BlockLength);
				if (array.Length != 0)
				{
					SignatureHelpers.WriteBlock(binaryWriter, array, settings);
				}
			}
			binaryWriter.Flush();
			bufferStream.Seek(0L, SeekOrigin.Begin);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long num = StreamHelpers.ComputeNewPosition(offset, origin, Length, Position);
			if (num < 12)
			{
				InitializeHeader();
				bufferStream.Seek(num, SeekOrigin.Begin);
			}
			else
			{
				long a = num - 12;
				int num2 = 4 + settings.StrongSumLength;
				long result;
				long num3 = Math.DivRem(a, num2, out result);
				inputStream.Seek(num3 * settings.BlockLength, SeekOrigin.Begin);
				FillBuffer();
				bufferStream.Seek(result, SeekOrigin.Begin);
			}
			currentPosition = num;
			return currentPosition;
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		private void InitializeHeader()
		{
			bufferStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(bufferStream);
			SignatureHelpers.WriteHeader(binaryWriter, settings);
			binaryWriter.Flush();
			bufferStream.Seek(0L, SeekOrigin.Begin);
		}
	}
}
