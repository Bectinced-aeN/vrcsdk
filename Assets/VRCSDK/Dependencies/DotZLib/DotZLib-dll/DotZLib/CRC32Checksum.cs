using System;
using System.Runtime.InteropServices;

namespace DotZLib
{
	public sealed class CRC32Checksum : ChecksumGeneratorBase
	{
		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint crc32(uint crc, int data, uint length);

		public CRC32Checksum()
		{
		}

		public CRC32Checksum(uint initialValue)
			: base(initialValue)
		{
		}

		public override void Update(byte[] data, int offset, int count)
		{
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (offset + count > data.Length)
			{
				throw new ArgumentException();
			}
			GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				_current = crc32(_current, gCHandle.AddrOfPinnedObject().ToInt32() + offset, (uint)count);
			}
			finally
			{
				gCHandle.Free();
			}
		}
	}
}
