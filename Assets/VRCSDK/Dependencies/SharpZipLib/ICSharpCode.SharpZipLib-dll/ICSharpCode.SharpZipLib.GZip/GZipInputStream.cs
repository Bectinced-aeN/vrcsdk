using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.GZip
{
	public class GZipInputStream : InflaterInputStream
	{
		protected Crc32 crc;

		private bool readGZIPHeader;

		private bool completedLastBlock;

		public GZipInputStream(Stream baseInputStream)
			: this(baseInputStream, 4096)
		{
		}

		public GZipInputStream(Stream baseInputStream, int size)
			: base(baseInputStream, new Inflater(noHeader: true), size)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num;
			do
			{
				if (!readGZIPHeader)
				{
					try
					{
						if (!ReadHeader())
						{
							return 0;
						}
					}
					catch (Exception ex) when (completedLastBlock && (ex is GZipException || ex is EndOfStreamException))
					{
						return 0;
					}
				}
				num = base.Read(buffer, offset, count);
				if (num > 0)
				{
					crc.Update(buffer, offset, num);
				}
				if (inf.IsFinished)
				{
					ReadFooter();
				}
			}
			while (num <= 0);
			return num;
		}

		private bool ReadHeader()
		{
			this.crc = new Crc32();
			if (inputBuffer.Available <= 0)
			{
				inputBuffer.Fill();
				if (inputBuffer.Available <= 0)
				{
					return false;
				}
			}
			Crc32 crc = new Crc32();
			int num = inputBuffer.ReadLeByte();
			if (num < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			crc.Update(num);
			if (num != 31)
			{
				throw new GZipException("Error GZIP header, first magic byte doesn't match");
			}
			num = inputBuffer.ReadLeByte();
			if (num < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			if (num != 139)
			{
				throw new GZipException("Error GZIP header,  second magic byte doesn't match");
			}
			crc.Update(num);
			int num2 = inputBuffer.ReadLeByte();
			if (num2 < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			if (num2 != 8)
			{
				throw new GZipException("Error GZIP header, data not in deflate format");
			}
			crc.Update(num2);
			int num3 = inputBuffer.ReadLeByte();
			if (num3 < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			crc.Update(num3);
			if ((num3 & 0xE0) != 0)
			{
				throw new GZipException("Reserved flag bits in GZIP header != 0");
			}
			for (int i = 0; i < 6; i++)
			{
				int num4 = inputBuffer.ReadLeByte();
				if (num4 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num4);
			}
			if ((num3 & 4) != 0)
			{
				int num5 = inputBuffer.ReadLeByte();
				int num6 = inputBuffer.ReadLeByte();
				if (num5 < 0 || num6 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num5);
				crc.Update(num6);
				int num7 = (num6 << 8) | num5;
				for (int j = 0; j < num7; j++)
				{
					int num8 = inputBuffer.ReadLeByte();
					if (num8 < 0)
					{
						throw new EndOfStreamException("EOS reading GZIP header");
					}
					crc.Update(num8);
				}
			}
			if ((num3 & 8) != 0)
			{
				int num9;
				while ((num9 = inputBuffer.ReadLeByte()) > 0)
				{
					crc.Update(num9);
				}
				if (num9 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num9);
			}
			if ((num3 & 0x10) != 0)
			{
				int num10;
				while ((num10 = inputBuffer.ReadLeByte()) > 0)
				{
					crc.Update(num10);
				}
				if (num10 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num10);
			}
			if ((num3 & 2) != 0)
			{
				int num11 = inputBuffer.ReadLeByte();
				if (num11 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				int num12 = inputBuffer.ReadLeByte();
				if (num12 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				if (((num11 << 8) | num12) != ((int)crc.Value & 0xFFFF))
				{
					throw new GZipException("Header CRC value mismatch");
				}
			}
			readGZIPHeader = true;
			return true;
		}

		private void ReadFooter()
		{
			byte[] array = new byte[8];
			long num = inf.TotalOut & uint.MaxValue;
			inputBuffer.Available += inf.RemainingInput;
			inf.Reset();
			int num3;
			for (int num2 = 8; num2 > 0; num2 -= num3)
			{
				num3 = inputBuffer.ReadClearTextBuffer(array, 8 - num2, num2);
				if (num3 <= 0)
				{
					throw new EndOfStreamException("EOS reading GZIP footer");
				}
			}
			int num4 = (array[0] & 0xFF) | ((array[1] & 0xFF) << 8) | ((array[2] & 0xFF) << 16) | (array[3] << 24);
			if (num4 != (int)crc.Value)
			{
				throw new GZipException("GZIP crc sum mismatch, theirs \"" + num4 + "\" and ours \"" + (int)crc.Value);
			}
			uint num5 = (uint)((array[4] & 0xFF) | ((array[5] & 0xFF) << 8) | ((array[6] & 0xFF) << 16) | (array[7] << 24));
			if (num != num5)
			{
				throw new GZipException("Number of bytes mismatch in footer");
			}
			readGZIPHeader = false;
			completedLastBlock = true;
		}
	}
}
