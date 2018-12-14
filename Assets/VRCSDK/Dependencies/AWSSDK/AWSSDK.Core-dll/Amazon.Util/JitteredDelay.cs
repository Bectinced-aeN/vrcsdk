using System;

namespace Amazon.Util
{
	public class JitteredDelay
	{
		private TimeSpan _maxDelay;

		private TimeSpan _variance;

		private TimeSpan _baseIncrement;

		private Random _rand;

		private int _count;

		public JitteredDelay(TimeSpan baseIncrement, TimeSpan variance)
			: this(baseIncrement, variance, new TimeSpan(0, 0, 30))
		{
		}

		public JitteredDelay(TimeSpan baseIncrement, TimeSpan variance, TimeSpan maxDelay)
		{
			_baseIncrement = baseIncrement;
			_variance = variance;
			_maxDelay = maxDelay;
			_rand = new Random();
		}

		public TimeSpan GetRetryDelay(int attemptCount)
		{
			return new TimeSpan(_baseIncrement.Ticks * (long)Math.Pow(2.0, (double)attemptCount) + (long)(_rand.NextDouble() * (double)_variance.Ticks));
		}

		public TimeSpan Next()
		{
			long ticks = GetRetryDelay(_count + 1).Ticks;
			if (ticks < _maxDelay.Ticks)
			{
				_count++;
			}
			else
			{
				ticks = _maxDelay.Ticks;
			}
			return new TimeSpan(ticks);
		}

		public void Reset()
		{
			_count = 0;
		}
	}
}
