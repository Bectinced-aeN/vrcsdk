using System;
using System.Runtime.InteropServices;

namespace DotZLib
{
	public sealed class AdlerChecksum : ChecksumGeneratorBase
	{
		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint adler32(uint adler, int data, uint length);

		public AdlerChecksum()
		{
		}

		public AdlerChecksum(uint initialValue)
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
				_current = adler32(_current, gCHandle.AddrOfPinnedObject().ToInt32() + offset, (uint)count);
			}
			finally
			{
				gCHandle.Free();
			}
		}
	}
}
