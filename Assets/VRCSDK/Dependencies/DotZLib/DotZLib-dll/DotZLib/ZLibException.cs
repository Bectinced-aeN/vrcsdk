using System;

namespace DotZLib
{
	public class ZLibException : ApplicationException
	{
		public ZLibException(int errorCode, string msg)
			: base($"ZLib error {errorCode} {msg}")
		{
		}

		public ZLibException(int errorCode)
			: base($"ZLib error {errorCode}")
		{
		}
	}
}
