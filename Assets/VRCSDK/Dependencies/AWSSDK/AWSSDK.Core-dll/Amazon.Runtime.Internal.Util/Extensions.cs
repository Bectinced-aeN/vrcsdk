using System;
using System.Diagnostics;

namespace Amazon.Runtime.Internal.Util
{
	public static class Extensions
	{
		private static readonly long ticksPerSecond = TimeSpan.FromSeconds(1.0).Ticks;

		private static readonly double tickFrequency = (double)ticksPerSecond / (double)Stopwatch.Frequency;

		public static long GetElapsedDateTimeTicks(this Stopwatch self)
		{
			return (long)((double)self.ElapsedTicks * tickFrequency);
		}

		public static bool HasRequestData(this IRequest request)
		{
			if (request == null)
			{
				return false;
			}
			if (request.ContentStream != null || request.Content != null)
			{
				return true;
			}
			if (request.Parameters.Count > 0)
			{
				return true;
			}
			return false;
		}
	}
}
