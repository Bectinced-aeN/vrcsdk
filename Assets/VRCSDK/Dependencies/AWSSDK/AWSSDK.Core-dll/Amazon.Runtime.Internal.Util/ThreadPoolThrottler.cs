using System;
using System.Collections.Generic;
using System.Threading;

namespace Amazon.Runtime.Internal.Util
{
	public class ThreadPoolThrottler<T>
	{
		public class ThreadPoolOptions<Q>
		{
			public Action<Q> Callback
			{
				get;
				set;
			}

			public Action<Exception, Q> ErrorCallback
			{
				get;
				set;
			}

			public Q State
			{
				get;
				set;
			}
		}

		private static object _queueLock = new object();

		private int _requestCount;

		private Queue<ThreadPoolOptions<T>> _queuedRequests = new Queue<ThreadPoolOptions<T>>();

		public int MaxConcurentRequest
		{
			get;
			private set;
		}

		public int RequestCount => Thread.VolatileRead(ref _requestCount);

		public ThreadPoolThrottler(int maxConcurrentRequests)
		{
			MaxConcurentRequest = maxConcurrentRequests;
		}

		public void Enqueue(T executionContext, Action<T> callback, Action<Exception, T> errorCallback)
		{
			ThreadPoolOptions<T> threadPoolOptions = new ThreadPoolOptions<T>
			{
				Callback = callback,
				ErrorCallback = errorCallback,
				State = executionContext
			};
			if (Interlocked.Increment(ref _requestCount) <= MaxConcurentRequest)
			{
				ThreadPool.QueueUserWorkItem(Callback, threadPoolOptions);
			}
			else
			{
				lock (_queueLock)
				{
					_queuedRequests.Enqueue(threadPoolOptions);
				}
			}
		}

		private void Callback(object state)
		{
			ThreadPoolOptions<T> threadPoolOptions = (ThreadPoolOptions<T>)state;
			try
			{
				threadPoolOptions.Callback(threadPoolOptions.State);
			}
			catch (Exception arg)
			{
				threadPoolOptions.ErrorCallback(arg, threadPoolOptions.State);
			}
			finally
			{
				SignalCompletion();
			}
		}

		private void SignalCompletion()
		{
			ThreadPoolOptions<T> state = null;
			int num = 0;
			lock (_queueLock)
			{
				num = _queuedRequests.Count;
				if (num > 0)
				{
					state = _queuedRequests.Dequeue();
				}
			}
			if (num > 0)
			{
				ThreadPool.QueueUserWorkItem(Callback, state);
			}
			Interlocked.Decrement(ref _requestCount);
		}
	}
}
