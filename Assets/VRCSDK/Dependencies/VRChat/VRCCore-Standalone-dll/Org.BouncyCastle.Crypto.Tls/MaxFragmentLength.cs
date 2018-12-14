namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class MaxFragmentLength
	{
		public const byte pow2_9 = 1;

		public const byte pow2_10 = 2;

		public const byte pow2_11 = 3;

		public const byte pow2_12 = 4;

		public static bool IsValid(byte maxFragmentLength)
		{
			return maxFragmentLength >= 1 && maxFragmentLength <= 4;
		}
	}
}
