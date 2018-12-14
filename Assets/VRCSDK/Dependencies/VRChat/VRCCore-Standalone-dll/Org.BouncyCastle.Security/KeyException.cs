using System;

namespace Org.BouncyCastle.Security
{
	[Serializable]
	internal class KeyException : GeneralSecurityException
	{
		public KeyException()
		{
		}

		public KeyException(string message)
			: base(message)
		{
		}

		public KeyException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
