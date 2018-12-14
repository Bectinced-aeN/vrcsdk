using System;

namespace Org.BouncyCastle.Crypto
{
	[Serializable]
	internal class OutputLengthException : DataLengthException
	{
		public OutputLengthException()
		{
		}

		public OutputLengthException(string message)
			: base(message)
		{
		}

		public OutputLengthException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
