using Amazon.Runtime.Internal.Util;
using System;
using System.Threading;

namespace Amazon.Runtime.Internal
{
	public class RuntimeAsyncResult : IAsyncResult, IDisposable
	{
		private object _lockObj;

		private ManualResetEvent _waitHandle;

		private bool _disposed;

		private bool _callbackInvoked;

		private ILogger _logger;

		private AsyncCallback AsyncCallback
		{
			get;
			set;
		}

		public object AsyncState
		{
			get;
			private set;
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (_waitHandle != null)
				{
					return _waitHandle;
				}
				lock (_lockObj)
				{
					if (_waitHandle == null)
					{
						_waitHandle = new ManualResetEvent(IsCompleted);
					}
				}
				return _waitHandle;
			}
		}

		public bool CompletedSynchronously
		{
			get;
			private set;
		}

		public bool IsCompleted
		{
			get;
			private set;
		}

		public Exception Exception
		{
			get;
			set;
		}

		public AmazonWebServiceResponse Response
		{
			get;
			set;
		}

		public AmazonWebServiceRequest Request
		{
			get;
			set;
		}

		public Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> Action
		{
			get;
			set;
		}

		public AsyncOptions AsyncOptions
		{
			get;
			set;
		}

		public RuntimeAsyncResult(AsyncCallback asyncCallback, object asyncState)
		{
			_lockObj = new object();
			_callbackInvoked = false;
			_logger = Logger.GetLogger(typeof(RuntimeAsyncResult));
			AsyncState = asyncState;
			IsCompleted = false;
			AsyncCallback = asyncCallback;
			CompletedSynchronously = false;
			_logger = Logger.GetLogger(GetType());
		}

		internal void SignalWaitHandle()
		{
			IsCompleted = true;
			if (_waitHandle != null)
			{
				_waitHandle.Set();
			}
		}

		internal void HandleException(Exception exception)
		{
			Exception = exception;
			InvokeCallback();
		}

		public void InvokeCallback()
		{
			SignalWaitHandle();
			if (!_callbackInvoked)
			{
				_callbackInvoked = true;
				try
				{
					if (AsyncOptions.ExecuteCallbackOnMainThread)
					{
						UnityRequestQueue.Instance.EnqueueCallback(this);
					}
					else if (Action != null)
					{
						Action(Request, Response, Exception, AsyncOptions);
					}
				}
				catch (Exception exception)
				{
					_logger.Error(exception, "An unhandled exception occurred in the user callback.");
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing && _waitHandle != null)
				{
					_waitHandle.Close();
					_waitHandle = null;
				}
				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
