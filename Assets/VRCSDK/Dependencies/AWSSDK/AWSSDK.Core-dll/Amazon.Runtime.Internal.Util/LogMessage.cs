using System;
using System.Globalization;

namespace Amazon.Runtime.Internal.Util
{
	public class LogMessage : ILogMessage
	{
		public object[] Args
		{
			get;
			private set;
		}

		public IFormatProvider Provider
		{
			get;
			private set;
		}

		public string Format
		{
			get;
			private set;
		}

		public LogMessage(string message)
			: this(CultureInfo.InvariantCulture, message)
		{
		}

		public LogMessage(string format, params object[] args)
			: this(CultureInfo.InvariantCulture, format, args)
		{
		}

		public LogMessage(IFormatProvider provider, string format, params object[] args)
		{
			Args = args;
			Format = format;
			Provider = provider;
		}

		public override string ToString()
		{
			return string.Format(Provider, Format, Args);
		}
	}
}
