using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.Util.Internal.PlatformServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;

namespace Amazon.Runtime.Internal
{
	public class UnityMainThreadDispatcher : MonoBehaviour
	{
		private Logger _logger;

		private float _nextUpdateTime;

		private float _updateInterval = 0.1f;

		private NetworkStatus _currentNetworkStatus;

		public void Awake()
		{
			_logger = Logger.GetLogger(base.GetType());
			_nextUpdateTime = Time.get_unscaledTime();
			_nextUpdateTime += _updateInterval;
		}

		private void Update()
		{
			if (Time.get_unscaledTime() >= _nextUpdateTime)
			{
				ProcessRequests();
				_nextUpdateTime += _updateInterval;
			}
		}

		private void ProcessRequests()
		{
			IUnityHttpRequest unityHttpRequest = UnityRequestQueue.Instance.DequeueRequest();
			if (unityHttpRequest != null)
			{
				this.StartCoroutine(InvokeRequest(unityHttpRequest));
			}
			RuntimeAsyncResult runtimeAsyncResult = UnityRequestQueue.Instance.DequeueCallback();
			if (runtimeAsyncResult != null && runtimeAsyncResult.Action != null)
			{
				try
				{
					runtimeAsyncResult.Action(runtimeAsyncResult.Request, runtimeAsyncResult.Response, runtimeAsyncResult.Exception, runtimeAsyncResult.AsyncOptions);
				}
				catch (Exception exception)
				{
					_logger.Error(exception, "An unhandled exception was thrown from the callback method {0}.", runtimeAsyncResult.Request.ToString());
				}
			}
			Action action = UnityRequestQueue.Instance.DequeueMainThreadOperation();
			if (action != null)
			{
				try
				{
					action();
				}
				catch (Exception exception2)
				{
					_logger.Error(exception2, "An unhandled exception was thrown from the callback method");
				}
			}
			NetworkReachability networkReachability = ServiceFactory.Instance.GetService<INetworkReachability>() as NetworkReachability;
			if (_currentNetworkStatus != networkReachability.NetworkStatus)
			{
				_currentNetworkStatus = networkReachability.NetworkStatus;
				networkReachability.OnNetworkReachabilityChanged(_currentNetworkStatus);
			}
		}

		private IEnumerator InvokeRequest(IUnityHttpRequest request)
		{
			if ((ServiceFactory.Instance.GetService<INetworkReachability>() as NetworkReachability).NetworkStatus != 0)
			{
				if (request is UnityWwwRequest)
				{
					WWW wwwRequest = new WWW((request as UnityWwwRequest).RequestUri.AbsoluteUri, request.RequestContent, request.Headers);
					if (wwwRequest == null)
					{
						yield return (object)null;
					}
					bool uploadCompleted2 = false;
					while (!wwwRequest.get_isDone())
					{
						float uploadProgress = wwwRequest.get_uploadProgress();
						if (!uploadCompleted2)
						{
							request.OnUploadProgressChanged(uploadProgress);
							if (uploadProgress == 1f)
							{
								uploadCompleted2 = true;
							}
						}
						yield return (object)null;
					}
					request.WwwRequest = (IDisposable)wwwRequest;
					request.Response = new UnityWebResponseData(wwwRequest);
				}
				else
				{
					UnityRequest unityRequest = request as UnityRequest;
					if (unityRequest == null)
					{
						yield return (object)null;
					}
					UnityWebRequestWrapper unityWebRequest = new UnityWebRequestWrapper(unityRequest.RequestUri.AbsoluteUri, unityRequest.Method)
					{
						DownloadHandler = new DownloadHandlerBufferWrapper()
					};
					if (request.RequestContent != null && request.RequestContent.Length != 0)
					{
						unityWebRequest.UploadHandler = new UploadHandlerRawWrapper(request.RequestContent);
					}
					bool uploadCompleted = false;
					foreach (KeyValuePair<string, string> header in request.Headers)
					{
						unityWebRequest.SetRequestHeader(header.Key, header.Value);
					}
					AsyncOperation operation = unityWebRequest.Send();
					while (!operation.get_isDone())
					{
						float progress = operation.get_progress();
						if (!uploadCompleted)
						{
							request.OnUploadProgressChanged(progress);
							if (progress == 1f)
							{
								uploadCompleted = true;
							}
						}
						yield return (object)null;
					}
					request.WwwRequest = unityWebRequest;
					request.Response = new UnityWebResponseData(unityWebRequest);
				}
			}
			else
			{
				request.Exception = new WebException("Network Unavailable", WebExceptionStatus.ConnectFailure);
			}
			if (request.IsSync)
			{
				if (request.Response != null && !request.Response.IsSuccessStatusCode)
				{
					request.Exception = new HttpErrorResponseException(request.Response);
				}
				request.WaitHandle.Set();
			}
			else
			{
				if (request.Response != null && !request.Response.IsSuccessStatusCode)
				{
					request.Exception = new HttpErrorResponseException(request.Response);
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					try
					{
						request.Callback(request.AsyncResult);
					}
					catch (Exception exception)
					{
						_logger.Error(exception, "An exception was thrown from the callback method executed fromUnityMainThreadDispatcher.InvokeRequest method.");
					}
				});
			}
		}

		public UnityMainThreadDispatcher()
			: this()
		{
		}
	}
}
