namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class ECPointFormat
	{
		public const byte uncompressed = 0;

		public const byte ansiX962_compressed_prime = 1;

		public const byte ansiX962_compressed_char2 = 2;
	}
}
