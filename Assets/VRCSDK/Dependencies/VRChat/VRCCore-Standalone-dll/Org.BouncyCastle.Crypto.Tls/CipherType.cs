namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class CipherType
	{
		public const int stream = 0;

		public const int block = 1;

		public const int aead = 2;
	}
}
