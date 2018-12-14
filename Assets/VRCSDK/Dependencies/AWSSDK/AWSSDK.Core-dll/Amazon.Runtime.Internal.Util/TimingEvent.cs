using System;

namespace Amazon.Runtime.Internal.Util
{
	public class TimingEvent : IDisposable
	{
		private Metric metric;

		private RequestMetrics metrics;

		private bool disposed;

		internal TimingEvent(RequestMetrics metrics, Metric metric)
		{
			this.metrics = metrics;
			this.metric = metric;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					metrics.StopEvent(metric);
				}
				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~TimingEvent()
		{
			Dispose(disposing: false);
		}
	}
}
