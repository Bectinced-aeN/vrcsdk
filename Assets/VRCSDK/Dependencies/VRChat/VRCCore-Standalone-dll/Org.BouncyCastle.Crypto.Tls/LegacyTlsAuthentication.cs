using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class LegacyTlsAuthentication : TlsAuthentication
	{
		protected ICertificateVerifyer verifyer;

		protected IClientCredentialsProvider credProvider;

		protected Uri TargetUri;

		public LegacyTlsAuthentication(Uri targetUri, ICertificateVerifyer verifyer, IClientCredentialsProvider prov)
		{
			TargetUri = targetUri;
			this.verifyer = verifyer;
			credProvider = prov;
		}

		public virtual void NotifyServerCertificate(Certificate serverCertificate)
		{
			if (!verifyer.IsValid(TargetUri, serverCertificate.GetCertificateList()))
			{
				throw new TlsFatalAlert(90);
			}
		}

		public virtual TlsCredentials GetClientCredentials(TlsContext context, CertificateRequest certificateRequest)
		{
			object result;
			if (credProvider == null)
			{
				TlsCredentials tlsCredentials = null;
				result = tlsCredentials;
			}
			else
			{
				result = credProvider.GetClientCredentials(context, certificateRequest);
			}
			return (TlsCredentials)result;
		}
	}
}
