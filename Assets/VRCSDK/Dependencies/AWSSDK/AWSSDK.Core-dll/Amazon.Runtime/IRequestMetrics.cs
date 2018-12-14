using System.Collections.Generic;

namespace Amazon.Runtime
{
	public interface IRequestMetrics
	{
		Dictionary<Metric, List<object>> Properties
		{
			get;
		}

		Dictionary<Metric, List<IMetricsTiming>> Timings
		{
			get;
		}

		Dictionary<Metric, long> Counters
		{
			get;
		}

		bool IsEnabled
		{
			get;
		}

		string ToJSON();
	}
}
