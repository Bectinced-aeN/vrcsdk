using System;

namespace ICSharpCode.SharpZipLib.Lzw
{
	public class LzwException : SharpZipBaseException
	{
		public LzwException()
		{
		}

		public LzwException(string message)
			: base(message)
		{
		}

		public LzwException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
