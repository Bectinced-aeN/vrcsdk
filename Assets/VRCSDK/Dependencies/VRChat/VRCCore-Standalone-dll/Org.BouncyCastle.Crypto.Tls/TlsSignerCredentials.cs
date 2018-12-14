namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface TlsSignerCredentials : TlsCredentials
	{
		SignatureAndHashAlgorithm SignatureAndHashAlgorithm
		{
			get;
		}

		byte[] GenerateCertificateSignature(byte[] hash);
	}
}
