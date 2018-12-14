using System;
using System.Collections.Generic;

namespace Amazon.Runtime.Internal
{
	public sealed class UnityRequestQueue
	{
		private static readonly UnityRequestQueue _instance = new UnityRequestQueue();

		private static readonly object _requestsLock = new object();

		private static readonly object _callbacksLock = new object();

		private static readonly object _mainThreadCallbackLock = new object();

		private Queue<IUnityHttpRequest> _requests = new Queue<IUnityHttpRequest>();

		private Queue<RuntimeAsyncResult> _callbacks = new Queue<RuntimeAsyncResult>();

		private Queue<Action> _mainThreadCallbacks = new Queue<Action>();

		public static UnityRequestQueue Instance => _instance;

		private UnityRequestQueue()
		{
		}

		public void EnqueueRequest(IUnityHttpRequest request)
		{
			lock (_requestsLock)
			{
				_requests.Enqueue(request);
			}
		}

		public IUnityHttpRequest DequeueRequest()
		{
			IUnityHttpRequest result = null;
			lock (_requestsLock)
			{
				if (_requests.Count > 0)
				{
					return _requests.Dequeue();
				}
				return result;
			}
		}

		public void EnqueueCallback(RuntimeAsyncResult asyncResult)
		{
			lock (_callbacksLock)
			{
				_callbacks.Enqueue(asyncResult);
			}
		}

		public RuntimeAsyncResult DequeueCallback()
		{
			RuntimeAsyncResult result = null;
			lock (_callbacksLock)
			{
				if (_callbacks.Count > 0)
				{
					return _callbacks.Dequeue();
				}
				return result;
			}
		}

		public void ExecuteOnMainThread(Action action)
		{
			lock (_mainThreadCallbackLock)
			{
				_mainThreadCallbacks.Enqueue(action);
			}
		}

		public Action DequeueMainThreadOperation()
		{
			Action result = null;
			lock (_mainThreadCallbackLock)
			{
				if (_mainThreadCallbacks.Count > 0)
				{
					return _mainThreadCallbacks.Dequeue();
				}
				return result;
			}
		}
	}
}
