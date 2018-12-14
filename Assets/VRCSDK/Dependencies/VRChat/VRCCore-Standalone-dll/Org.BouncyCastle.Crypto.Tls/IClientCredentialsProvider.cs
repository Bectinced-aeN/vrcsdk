namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface IClientCredentialsProvider
	{
		TlsCredentials GetClientCredentials(TlsContext context, CertificateRequest certificateRequest);
	}
}
