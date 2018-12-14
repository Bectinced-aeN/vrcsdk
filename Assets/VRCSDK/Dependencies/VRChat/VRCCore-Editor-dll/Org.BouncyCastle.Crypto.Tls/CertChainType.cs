namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class CertChainType
	{
		public const byte individual_certs = 0;

		public const byte pkipath = 1;

		public static bool IsValid(byte certChainType)
		{
			return certChainType >= 0 && certChainType <= 1;
		}
	}
}
