using System;
using System.Threading;

namespace Amazon.S3.Util
{
	public class AsyncCancelableResult : IAsyncCancelableResult, IAsyncResult, IDisposable
	{
		private volatile bool _isCancelRequested;

		private volatile bool _isCompleted;

		private volatile bool _isCanceled;

		private ManualResetEvent _waitHandle;

		public bool IsCanceled => _isCanceled;

		public object AsyncState
		{
			get;
			private set;
		}

		public WaitHandle AsyncWaitHandle => _waitHandle;

		public bool CompletedSynchronously => false;

		public bool IsCompleted => _isCompleted;

		internal bool IsCancelRequested => _isCancelRequested;

		internal Exception LastException
		{
			get;
			set;
		}

		internal AsyncCallback Callback
		{
			get;
			set;
		}

		internal AsyncCancelableResult(AsyncCallback callback, object state)
		{
			Callback = callback;
			AsyncState = state;
			_waitHandle = new ManualResetEvent(initialState: false);
		}

		public void Cancel()
		{
			_isCancelRequested = true;
		}

		internal void SignalWaitHandleOnCanceled()
		{
			_isCanceled = true;
			_waitHandle.Set();
			if (Callback != null)
			{
				Callback(this);
			}
		}

		internal void SignalWaitHandleOnCompleted()
		{
			_isCompleted = true;
			_waitHandle.Set();
			if (Callback != null)
			{
				Callback(this);
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _waitHandle != null)
			{
				_waitHandle.Close();
				_waitHandle = null;
			}
		}
	}
}
