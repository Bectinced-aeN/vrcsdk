namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface TlsAuthentication
	{
		void NotifyServerCertificate(Certificate serverCertificate);

		TlsCredentials GetClientCredentials(TlsContext context, CertificateRequest certificateRequest);
	}
}
