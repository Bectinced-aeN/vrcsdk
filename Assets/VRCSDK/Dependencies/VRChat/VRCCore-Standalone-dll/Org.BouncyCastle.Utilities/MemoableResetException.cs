using System;

namespace Org.BouncyCastle.Utilities
{
	internal class MemoableResetException : InvalidCastException
	{
		public MemoableResetException(string msg)
			: base(msg)
		{
		}
	}
}
