using System.Linq;

namespace Amazon.Runtime.Internal.Util
{
	public static class Hashing
	{
		public static int Hash(params object[] value)
		{
			return CombineHashes((from v in value
			select v?.GetHashCode() ?? 0).ToArray());
		}

		public static int CombineHashes(params int[] hashes)
		{
			int num = 0;
			foreach (int b in hashes)
			{
				num = CombineHashesInternal(num, b);
			}
			return num;
		}

		private static int CombineHashesInternal(int a, int b)
		{
			return ((a << 5) + a) ^ b;
		}
	}
}
