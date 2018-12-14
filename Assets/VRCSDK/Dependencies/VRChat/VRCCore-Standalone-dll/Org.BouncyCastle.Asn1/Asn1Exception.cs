using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	[Serializable]
	internal class Asn1Exception : IOException
	{
		public Asn1Exception()
		{
		}

		public Asn1Exception(string message)
			: base(message)
		{
		}

		public Asn1Exception(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
