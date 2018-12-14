using Amazon.Runtime;
using System;

namespace Amazon.Util
{
	public class LoggingConfig
	{
		public static readonly int DefaultLogResponsesSizeLimit = 1024;

		private LoggingOptions _logTo;

		public LoggingOptions LogTo
		{
			get
			{
				return _logTo;
			}
			set
			{
				_logTo = value;
				AWSConfigs.OnPropertyChanged("LogTo");
			}
		}

		public ResponseLoggingOption LogResponses
		{
			get;
			set;
		}

		public int LogResponsesSizeLimit
		{
			get;
			set;
		}

		public bool LogMetrics
		{
			get;
			set;
		}

		public LogMetricsFormatOption LogMetricsFormat
		{
			get;
			set;
		}

		public IMetricsFormatter LogMetricsCustomFormatter
		{
			get;
			set;
		}

		internal LoggingConfig()
		{
			LogTo = AWSConfigs._logging;
			LogResponses = AWSConfigs._responseLogging;
			LogResponsesSizeLimit = DefaultLogResponsesSizeLimit;
			LogMetrics = AWSConfigs._logMetrics;
		}

		internal void Configure(LoggingSection section)
		{
			if (section != null)
			{
				LogTo = section.LogTo;
				LogResponses = section.LogResponses;
				LogMetrics = section.LogMetrics.GetValueOrDefault(false);
				LogMetricsFormat = section.LogMetricsFormat;
				LogResponsesSizeLimit = (section.LogResponsesSizeLimit.HasValue ? section.LogResponsesSizeLimit.Value : 1024);
				if (section.LogMetricsCustomFormatter != null && typeof(IMetricsFormatter).IsAssignableFrom(section.LogMetricsCustomFormatter))
				{
					LogMetricsCustomFormatter = (Activator.CreateInstance(section.LogMetricsCustomFormatter) as IMetricsFormatter);
				}
			}
		}
	}
}
