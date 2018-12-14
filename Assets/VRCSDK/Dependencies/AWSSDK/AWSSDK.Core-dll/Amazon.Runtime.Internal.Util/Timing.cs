using System;

namespace Amazon.Runtime.Internal.Util
{
	public class Timing : IMetricsTiming
	{
		private long startTime;

		private long endTime;

		public bool IsFinished
		{
			get;
			private set;
		}

		public long ElapsedTicks
		{
			get
			{
				if (IsFinished)
				{
					return endTime - startTime;
				}
				return 0L;
			}
		}

		public TimeSpan ElapsedTime => TimeSpan.FromTicks(ElapsedTicks);

		public Timing()
		{
			startTime = (endTime = 0L);
			IsFinished = true;
		}

		public Timing(long currentTime)
		{
			startTime = currentTime;
			IsFinished = false;
		}

		public void Stop(long currentTime)
		{
			endTime = currentTime;
			IsFinished = true;
		}
	}
}
