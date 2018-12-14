using System;

namespace Org.BouncyCastle.Security.Certificates
{
	[Serializable]
	internal class CertificateEncodingException : CertificateException
	{
		public CertificateEncodingException()
		{
		}

		public CertificateEncodingException(string msg)
			: base(msg)
		{
		}

		public CertificateEncodingException(string msg, Exception e)
			: base(msg, e)
		{
		}
	}
}
