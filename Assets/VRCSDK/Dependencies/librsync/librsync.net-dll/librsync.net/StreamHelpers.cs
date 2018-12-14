using System;
using System.IO;

namespace librsync.net
{
	public static class StreamHelpers
	{
		public static void WriteBigEndian(BinaryWriter s, ulong value, int bytes = 4)
		{
			s.Write(new byte[8]
			{
				(byte)(value >> 56),
				(byte)(value >> 48),
				(byte)(value >> 40),
				(byte)(value >> 32),
				(byte)(value >> 24),
				(byte)(value >> 16),
				(byte)(value >> 8),
				(byte)value
			}, 8 - bytes, bytes);
		}

		public static uint ReadBigEndianUint32(BinaryReader s)
		{
			return (uint)ConvertFromBigEndian(s.ReadBytes(4));
		}

		public static long ConvertFromBigEndian(byte[] bytes)
		{
			long num = 0L;
			for (int i = 0; i < bytes.Length; i++)
			{
				num = ((num << 8) | bytes[i]);
			}
			return num;
		}

		public static long ComputeNewPosition(long offset, SeekOrigin origin, long length, long currentPosition)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				return offset;
			case SeekOrigin.Current:
				return currentPosition + offset;
			case SeekOrigin.End:
				return length + offset;
			default:
				throw new ArgumentException("Invalid SeekOrigin");
			}
		}
	}
}
