using System;

namespace Org.BouncyCastle.Security.Certificates
{
	[Serializable]
	internal class CrlException : GeneralSecurityException
	{
		public CrlException()
		{
		}

		public CrlException(string msg)
			: base(msg)
		{
		}

		public CrlException(string msg, Exception e)
			: base(msg, e)
		{
		}
	}
}
