using System;

namespace Amazon.Runtime
{
	public interface IMetricsTiming
	{
		bool IsFinished
		{
			get;
		}

		long ElapsedTicks
		{
			get;
		}

		TimeSpan ElapsedTime
		{
			get;
		}
	}
}
