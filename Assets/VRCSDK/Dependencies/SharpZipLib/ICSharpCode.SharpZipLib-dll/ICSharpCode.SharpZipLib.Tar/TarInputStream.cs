using System;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarInputStream : Stream
	{
		public interface IEntryFactory
		{
			TarEntry CreateEntry(string name);

			TarEntry CreateEntryFromFile(string fileName);

			TarEntry CreateEntry(byte[] headerBuffer);
		}

		public class EntryFactoryAdapter : IEntryFactory
		{
			public TarEntry CreateEntry(string name)
			{
				return TarEntry.CreateTarEntry(name);
			}

			public TarEntry CreateEntryFromFile(string fileName)
			{
				return TarEntry.CreateEntryFromFile(fileName);
			}

			public TarEntry CreateEntry(byte[] headerBuffer)
			{
				return new TarEntry(headerBuffer);
			}
		}

		protected bool hasHitEOF;

		protected long entrySize;

		protected long entryOffset;

		protected byte[] readBuffer;

		protected TarBuffer tarBuffer;

		private TarEntry currentEntry;

		protected IEntryFactory entryFactory;

		private readonly Stream inputStream;

		public bool IsStreamOwner
		{
			get
			{
				return tarBuffer.IsStreamOwner;
			}
			set
			{
				tarBuffer.IsStreamOwner = value;
			}
		}

		public override bool CanRead => inputStream.CanRead;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => inputStream.Length;

		public override long Position
		{
			get
			{
				return inputStream.Position;
			}
			set
			{
				throw new NotSupportedException("TarInputStream Seek not supported");
			}
		}

		public int RecordSize => tarBuffer.RecordSize;

		public long Available => entrySize - entryOffset;

		public bool IsMarkSupported => false;

		public TarInputStream(Stream inputStream)
			: this(inputStream, 20)
		{
		}

		public TarInputStream(Stream inputStream, int blockFactor)
		{
			this.inputStream = inputStream;
			tarBuffer = TarBuffer.CreateInputTarBuffer(inputStream, blockFactor);
		}

		public override void Flush()
		{
			inputStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("TarInputStream Seek not supported");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("TarInputStream SetLength not supported");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("TarInputStream Write not supported");
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("TarInputStream WriteByte not supported");
		}

		public override int ReadByte()
		{
			byte[] array = new byte[1];
			if (Read(array, 0, 1) <= 0)
			{
				return -1;
			}
			return array[0];
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = 0;
			if (entryOffset >= entrySize)
			{
				return 0;
			}
			long num2 = count;
			if (num2 + entryOffset > entrySize)
			{
				num2 = entrySize - entryOffset;
			}
			if (readBuffer != null)
			{
				int num3 = (int)((num2 > readBuffer.Length) ? readBuffer.Length : num2);
				Array.Copy(readBuffer, 0, buffer, offset, num3);
				if (num3 >= readBuffer.Length)
				{
					readBuffer = null;
				}
				else
				{
					int num4 = readBuffer.Length - num3;
					byte[] destinationArray = new byte[num4];
					Array.Copy(readBuffer, num3, destinationArray, 0, num4);
					readBuffer = destinationArray;
				}
				num += num3;
				num2 -= num3;
				offset += num3;
			}
			while (num2 > 0)
			{
				byte[] array = tarBuffer.ReadBlock();
				if (array == null)
				{
					throw new TarException("unexpected EOF with " + num2 + " bytes unread");
				}
				int num5 = (int)num2;
				int num6 = array.Length;
				if (num6 > num5)
				{
					Array.Copy(array, 0, buffer, offset, num5);
					readBuffer = new byte[num6 - num5];
					Array.Copy(array, num5, readBuffer, 0, num6 - num5);
				}
				else
				{
					num5 = num6;
					Array.Copy(array, 0, buffer, offset, num6);
				}
				num += num5;
				num2 -= num5;
				offset += num5;
			}
			entryOffset += num;
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				tarBuffer.Close();
			}
		}

		public void SetEntryFactory(IEntryFactory factory)
		{
			entryFactory = factory;
		}

		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return tarBuffer.RecordSize;
		}

		public void Skip(long skipCount)
		{
			byte[] array = new byte[8192];
			int num2;
			for (long num = skipCount; num > 0; num -= num2)
			{
				int count = (int)((num > array.Length) ? array.Length : num);
				num2 = Read(array, 0, count);
				if (num2 == -1)
				{
					break;
				}
			}
		}

		public void Mark(int markLimit)
		{
		}

		public void Reset()
		{
		}

		public TarEntry GetNextEntry()
		{
			if (hasHitEOF)
			{
				return null;
			}
			if (currentEntry != null)
			{
				SkipToNextEntry();
			}
			byte[] array = tarBuffer.ReadBlock();
			if (array == null)
			{
				hasHitEOF = true;
			}
			else
			{
				hasHitEOF |= TarBuffer.IsEndOfArchiveBlock(array);
			}
			if (!hasHitEOF)
			{
				try
				{
					TarHeader tarHeader = new TarHeader();
					tarHeader.ParseBuffer(array);
					if (!tarHeader.IsChecksumValid)
					{
						throw new TarException("Header checksum is invalid");
					}
					entryOffset = 0L;
					entrySize = tarHeader.Size;
					StringBuilder stringBuilder = null;
					if (tarHeader.TypeFlag == 76)
					{
						byte[] array2 = new byte[512];
						long num = entrySize;
						stringBuilder = new StringBuilder();
						while (num > 0)
						{
							int num2 = Read(array2, 0, (int)((num > array2.Length) ? array2.Length : num));
							if (num2 == -1)
							{
								throw new InvalidHeaderException("Failed to read long name entry");
							}
							stringBuilder.Append(TarHeader.ParseName(array2, 0, num2).ToString());
							num -= num2;
						}
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag == 103)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag == 120)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag == 86)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag != 48 && tarHeader.TypeFlag != 0 && tarHeader.TypeFlag != 49 && tarHeader.TypeFlag != 50 && tarHeader.TypeFlag != 53)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					if (entryFactory == null)
					{
						currentEntry = new TarEntry(array);
						if (stringBuilder != null)
						{
							currentEntry.Name = stringBuilder.ToString();
						}
					}
					else
					{
						currentEntry = entryFactory.CreateEntry(array);
					}
					entryOffset = 0L;
					entrySize = currentEntry.Size;
				}
				catch (InvalidHeaderException ex)
				{
					entrySize = 0L;
					entryOffset = 0L;
					currentEntry = null;
					throw new InvalidHeaderException($"Bad header in record {tarBuffer.CurrentRecord} block {tarBuffer.CurrentBlock} {ex.Message}");
				}
			}
			else
			{
				currentEntry = null;
			}
			return currentEntry;
		}

		public void CopyEntryContents(Stream outputStream)
		{
			byte[] array = new byte[32768];
			while (true)
			{
				int num = Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				outputStream.Write(array, 0, num);
			}
		}

		private void SkipToNextEntry()
		{
			long num = entrySize - entryOffset;
			if (num > 0)
			{
				Skip(num);
			}
			readBuffer = null;
		}
	}
}
