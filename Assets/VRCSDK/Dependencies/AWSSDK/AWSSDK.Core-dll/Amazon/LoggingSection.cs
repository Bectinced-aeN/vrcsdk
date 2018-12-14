using System;

namespace Amazon
{
	internal class LoggingSection
	{
		public const string logToKey = "logTo";

		public const string logResponsesKey = "logResponses";

		public const string logMetricsKey = "logMetrics";

		public const string logMetricsFormatKey = "logMetricsFormat";

		public const string logMetricsCustomFormatterKey = "logMetricsCustomFormatter";

		public const string logResponsesSizeLimitKey = "logResponsesSizeLimit";

		public LoggingOptions LogTo
		{
			get;
			set;
		}

		public ResponseLoggingOption LogResponses
		{
			get;
			set;
		}

		public int? LogResponsesSizeLimit
		{
			get;
			set;
		}

		public bool? LogMetrics
		{
			get;
			set;
		}

		public LogMetricsFormatOption LogMetricsFormat
		{
			get;
			set;
		}

		public Type LogMetricsCustomFormatter
		{
			get;
			set;
		}
	}
}
