using System;

namespace Amazon.Runtime.Internal.Util
{
	public interface ILogger
	{
		void InfoFormat(string messageFormat, params object[] args);

		void Debug(Exception exception, string messageFormat, params object[] args);

		void DebugFormat(string messageFormat, params object[] args);

		void Error(Exception exception, string messageFormat, params object[] args);

		void Flush();
	}
}
