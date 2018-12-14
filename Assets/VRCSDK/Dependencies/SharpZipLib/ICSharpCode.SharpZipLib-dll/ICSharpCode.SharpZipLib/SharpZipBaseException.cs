using System;

namespace ICSharpCode.SharpZipLib
{
	public class SharpZipBaseException : Exception
	{
		public SharpZipBaseException()
		{
		}

		public SharpZipBaseException(string message)
			: base(message)
		{
		}

		public SharpZipBaseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
