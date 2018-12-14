namespace librsync.net
{
	internal class Rollsum
	{
		private const byte RS_CHAR_OFFSET = 31;

		private ulong s1;

		private ulong s2;

		public ulong Count
		{
			get;
			private set;
		}

		public int Digest => (int)((s2 << 16) | (s1 & 0xFFFF));

		public void Update(byte[] buf)
		{
			ulong num = s1;
			ulong num2 = s2;
			Count += (ulong)buf.Length;
			int i;
			for (i = 0; i < buf.Length - 4; i += 4)
			{
				num2 += 4 * (num + buf[i]) + (uint)(3 * buf[i + 1]) + (uint)(2 * buf[i + 2]) + buf[i + 3] + 310;
				num += (uint)(buf[i] + buf[i + 1] + buf[i + 2] + buf[i + 3] + 124);
			}
			for (; i < buf.Length; i++)
			{
				num = (ulong)((long)num + (long)(buf[i] + 31));
				num2 += num;
			}
			s1 = num;
			s2 = num2;
		}

		public void Rotate(byte byteOut, byte byteIn)
		{
			s1 += (ulong)(byteIn - byteOut);
			s2 += (ulong)((long)s1 - (long)Count * (long)(byteOut + 31));
		}

		public void Rollout(byte byteOut)
		{
			s1 -= (ulong)(byteOut - 31);
			s2 -= (ulong)((long)Count * (long)(byteOut * 31));
			Count--;
		}
	}
}
