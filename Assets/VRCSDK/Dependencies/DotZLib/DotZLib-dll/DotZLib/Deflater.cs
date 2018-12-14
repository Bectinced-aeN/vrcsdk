using System;
using System.Runtime.InteropServices;

namespace DotZLib
{
	public sealed class Deflater : CodecBase
	{
		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern int deflateInit_(ref ZStream sz, int level, string vs, int size);

		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int deflate(ref ZStream sz, int flush);

		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int deflateReset(ref ZStream sz);

		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int deflateEnd(ref ZStream sz);

		public Deflater(CompressLevel level)
		{
			int num = deflateInit_(ref _ztream, (int)level, Info.Version, Marshal.SizeOf(_ztream));
			if (num != 0)
			{
				throw new ZLibException(num, "Could not initialize deflater");
			}
			resetOutput();
		}

		public override void Add(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException();
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (offset + count > data.Length)
			{
				throw new ArgumentException();
			}
			int num = offset;
			int num2 = 0;
			while (num2 >= 0 && num < count)
			{
				copyInput(data, num, Math.Min(count - num, 16384));
				while (num2 >= 0 && _ztream.avail_in != 0)
				{
					num2 = deflate(ref _ztream, 0);
					if (num2 == 0)
					{
						while (_ztream.avail_out == 0)
						{
							OnDataAvailable();
							num2 = deflate(ref _ztream, 0);
						}
					}
					num += (int)_ztream.total_in;
				}
			}
			setChecksum(_ztream.adler);
		}

		public override void Finish()
		{
			int num;
			do
			{
				num = deflate(ref _ztream, 4);
				OnDataAvailable();
			}
			while (num == 0);
			setChecksum(_ztream.adler);
			deflateReset(ref _ztream);
			resetOutput();
		}

		protected override void CleanUp()
		{
			deflateEnd(ref _ztream);
		}
	}
}
