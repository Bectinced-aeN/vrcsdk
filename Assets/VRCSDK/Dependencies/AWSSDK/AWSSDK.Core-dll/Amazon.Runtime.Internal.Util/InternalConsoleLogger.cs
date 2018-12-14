using System;
using System.Globalization;
using System.Threading;

namespace Amazon.Runtime.Internal.Util
{
	internal class InternalConsoleLogger : InternalLogger
	{
		private enum LogLevel
		{
			Verbose = 2,
			Debug,
			Info,
			Warn,
			Error,
			Assert
		}

		public static long _sequanceId;

		public InternalConsoleLogger(Type declaringType)
			: base(declaringType)
		{
		}

		public override void Flush()
		{
		}

		public override void Error(Exception exception, string messageFormat, params object[] args)
		{
			Log(LogLevel.Error, string.Format(CultureInfo.CurrentCulture, messageFormat, args), exception);
		}

		public override void Debug(Exception exception, string messageFormat, params object[] args)
		{
			Log(LogLevel.Debug, string.Format(CultureInfo.CurrentCulture, messageFormat, args), exception);
		}

		public override void DebugFormat(string message, params object[] arguments)
		{
			Log(LogLevel.Debug, string.Format(CultureInfo.CurrentCulture, message, arguments), null);
		}

		public override void InfoFormat(string message, params object[] arguments)
		{
			Log(LogLevel.Info, string.Format(CultureInfo.CurrentCulture, message, arguments), null);
		}

		private void Log(LogLevel logLevel, string message, Exception ex)
		{
			string text = null;
			long num = Interlocked.Increment(ref _sequanceId);
			string text2 = DateTime.Now.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
			string text3 = logLevel.ToString().ToUpper(CultureInfo.InvariantCulture);
			Console.WriteLine(arg1: (ex == null) ? string.Format(CultureInfo.CurrentCulture, "{0}|{1}|{2}|{3}", num, text2, text3, message) : string.Format(CultureInfo.CurrentCulture, "{0}|{1}|{2}|{3} --> {4}", num, text2, text3, message, ex.ToString()), format: "{0} {1}", arg0: base.DeclaringType.Name);
		}
	}
}
