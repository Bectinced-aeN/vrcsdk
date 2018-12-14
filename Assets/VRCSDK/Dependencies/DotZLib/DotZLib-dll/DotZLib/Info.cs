using System.Runtime.InteropServices;

namespace DotZLib
{
	public class Info
	{
		private uint _flags;

		public bool HasDebugInfo => (_flags & 0x100) != 0;

		public bool UsesAssemblyCode => (_flags & 0x200) != 0;

		public int SizeOfUInt => bitSize(_flags & 3);

		public int SizeOfULong => bitSize((_flags >> 2) & 3);

		public int SizeOfPointer => bitSize((_flags >> 4) & 3);

		public int SizeOfOffset => bitSize((_flags >> 6) & 3);

		public static string Version => zlibVersion();

		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint zlibCompileFlags();

		[DllImport("ZLIB1.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern string zlibVersion();

		private static int bitSize(uint bits)
		{
			switch (bits)
			{
			case 0u:
				return 16;
			case 1u:
				return 32;
			case 2u:
				return 64;
			default:
				return -1;
			}
		}

		public Info()
		{
			_flags = zlibCompileFlags();
		}
	}
}
