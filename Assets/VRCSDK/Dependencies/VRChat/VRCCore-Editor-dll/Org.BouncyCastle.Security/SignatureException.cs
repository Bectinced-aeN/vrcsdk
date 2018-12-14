using System;

namespace Org.BouncyCastle.Security
{
	[Serializable]
	internal class SignatureException : GeneralSecurityException
	{
		public SignatureException()
		{
		}

		public SignatureException(string message)
			: base(message)
		{
		}

		public SignatureException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
