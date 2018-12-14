using System;
using System.Collections.Generic;
using System.IO;

namespace librsync.net
{
	internal class DeltaStream : Stream
	{
		private IEnumerator<OutputCommand> commandsToOutput;

		private MemoryStream currentCommandStream;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public DeltaStream(Stream signatureStream, Stream inputStream)
		{
			SignatureFile signatures = SignatureHelpers.ParseSignatureFile(signatureStream);
			IEnumerable<OutputCommand> enumerable = DeltaCalculator.ComputeCommands(new BinaryReader(inputStream), signatures);
			commandsToOutput = enumerable.GetEnumerator();
			currentCommandStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(currentCommandStream);
			StreamHelpers.WriteBigEndian(binaryWriter, 1920139830uL);
			binaryWriter.Flush();
			currentCommandStream.Seek(0L, SeekOrigin.Begin);
		}

		private int ReadInternalSync(byte[] buffer, int offset, int count)
		{
			int i;
			int num;
			for (i = 0; i < count; i += num)
			{
				if (currentCommandStream.Position == currentCommandStream.Length)
				{
					currentCommandStream = new MemoryStream();
					BinaryWriter binaryWriter = new BinaryWriter(currentCommandStream);
					while (currentCommandStream.Length < count - i && commandsToOutput.MoveNext())
					{
						DeltaCalculator.WriteCommand(binaryWriter, commandsToOutput.Current);
					}
					binaryWriter.Flush();
					currentCommandStream.Seek(0L, SeekOrigin.Begin);
				}
				num = currentCommandStream.Read(buffer, offset + i, count - i);
				if (num <= 0)
				{
					break;
				}
			}
			return i;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ReadInternalSync(buffer, offset, count);
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
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
	}
}
