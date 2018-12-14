using System.IO;

namespace ThirdParty.Ionic.Zlib
{
	internal class CRC32
	{
		private long _TotalBytesRead;

		private static uint[] crc32Table;

		private const int BUFFER_SIZE = 8192;

		private uint _RunningCrc32Result = uint.MaxValue;

		public long TotalBytesRead => _TotalBytesRead;

		public int Crc32Result => (int)(~_RunningCrc32Result);

		public int GetCrc32(Stream input)
		{
			return GetCrc32AndCopy(input, null);
		}

		public int GetCrc32AndCopy(Stream input, Stream output)
		{
			byte[] array = new byte[8192];
			int count = 8192;
			_TotalBytesRead = 0L;
			int num = input.Read(array, 0, count);
			output?.Write(array, 0, num);
			_TotalBytesRead += num;
			while (num > 0)
			{
				SlurpBlock(array, 0, num);
				num = input.Read(array, 0, count);
				output?.Write(array, 0, num);
				_TotalBytesRead += num;
			}
			return (int)(~_RunningCrc32Result);
		}

		public int ComputeCrc32(int W, byte B)
		{
			return _InternalComputeCrc32((uint)W, B);
		}

		internal int _InternalComputeCrc32(uint W, byte B)
		{
			return (int)(crc32Table[(W ^ B) & 0xFF] ^ (W >> 8));
		}

		public void SlurpBlock(byte[] block, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				int num = offset + i;
				_RunningCrc32Result = ((_RunningCrc32Result >> 8) ^ crc32Table[block[num] ^ (_RunningCrc32Result & 0xFF)]);
			}
			_TotalBytesRead += count;
		}

		static CRC32()
		{
			uint num = 3988292384u;
			crc32Table = new uint[256];
			for (uint num2 = 0u; num2 < 256; num2++)
			{
				uint num3 = num2;
				for (uint num4 = 8u; num4 != 0; num4--)
				{
					num3 = (((num3 & 1) != 1) ? (num3 >> 1) : ((num3 >> 1) ^ num));
				}
				crc32Table[num2] = num3;
			}
		}
	}
}
