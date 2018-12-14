using System;
using System.Globalization;

namespace Amazon.Runtime.Internal.Util
{
	public class MetricError
	{
		public Metric Metric
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public Exception Exception
		{
			get;
			private set;
		}

		public DateTime Time
		{
			get;
			private set;
		}

		public MetricError(Metric metric, string messageFormat, params object[] args)
			: this(metric, null, messageFormat, args)
		{
		}

		public MetricError(Metric metric, Exception exception, string messageFormat, params object[] args)
		{
			Time = DateTime.Now;
			try
			{
				Message = string.Format(CultureInfo.InvariantCulture, messageFormat, args);
			}
			catch
			{
				Message = string.Format(CultureInfo.InvariantCulture, "Error message: {0}", messageFormat);
			}
			Exception = exception;
			Metric = metric;
		}
	}
}
