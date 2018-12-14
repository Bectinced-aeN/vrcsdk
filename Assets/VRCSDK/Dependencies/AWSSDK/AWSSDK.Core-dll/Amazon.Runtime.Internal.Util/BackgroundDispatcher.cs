using System;
using System.Collections.Generic;
using System.Threading;

namespace Amazon.Runtime.Internal.Util
{
	internal class BackgroundDispatcher<T> : IDisposable
	{
		private bool isDisposed;

		private Action<T> action;

		private Queue<T> queue;

		private Thread backgroundThread;

		private AutoResetEvent resetEvent;

		private bool shouldStop;

		private const int MAX_QUEUE_SIZE = 100;

		public bool IsRunning
		{
			get;
			private set;
		}

		public BackgroundDispatcher(Action<T> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			queue = new Queue<T>();
			resetEvent = new AutoResetEvent(initialState: false);
			shouldStop = false;
			this.action = action;
			backgroundThread = new Thread(Run);
			backgroundThread.IsBackground = true;
			backgroundThread.Start();
		}

		~BackgroundDispatcher()
		{
			Stop();
			Dispose(disposing: false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing && resetEvent != null)
				{
					resetEvent.Close();
					resetEvent = null;
				}
				isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Dispatch(T data)
		{
			if (queue.Count < 100)
			{
				lock (queue)
				{
					if (queue.Count < 100)
					{
						queue.Enqueue(data);
					}
				}
			}
			resetEvent.Set();
		}

		public void Stop()
		{
			shouldStop = true;
			resetEvent.Set();
		}

		private void Run()
		{
			IsRunning = true;
			while (!shouldStop)
			{
				HandleInvoked();
				resetEvent.WaitOne();
			}
			HandleInvoked();
			IsRunning = false;
		}

		private void HandleInvoked()
		{
			while (true)
			{
				bool flag = false;
				T obj = default(T);
				lock (queue)
				{
					if (queue.Count > 0)
					{
						obj = queue.Dequeue();
						flag = true;
					}
				}
				if (!flag)
				{
					break;
				}
				try
				{
					action(obj);
				}
				catch
				{
				}
			}
		}
	}
}
