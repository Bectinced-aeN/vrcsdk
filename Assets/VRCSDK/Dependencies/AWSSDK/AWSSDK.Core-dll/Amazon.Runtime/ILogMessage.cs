using System;

namespace Amazon.Runtime
{
	public interface ILogMessage
	{
		string Format
		{
			get;
		}

		object[] Args
		{
			get;
		}

		IFormatProvider Provider
		{
			get;
		}
	}
}
