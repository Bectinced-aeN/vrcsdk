using System;
using System.Runtime.InteropServices;

namespace DotZLib
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct ZStream
	{
		public IntPtr next_in;

		public uint avail_in;

		public uint total_in;

		public IntPtr next_out;

		public uint avail_out;

		public uint total_out;

		[MarshalAs(UnmanagedType.LPStr)]
		private string msg;

		private uint state;

		private uint zalloc;

		private uint zfree;

		private uint opaque;

		private int data_type;

		public uint adler;

		private uint reserved;
	}
}
