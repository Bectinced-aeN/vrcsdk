using System;

namespace Org.BouncyCastle.Security.Certificates
{
	[Serializable]
	internal class CertificateParsingException : CertificateException
	{
		public CertificateParsingException()
		{
		}

		public CertificateParsingException(string message)
			: base(message)
		{
		}

		public CertificateParsingException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
