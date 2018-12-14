using Amazon.Runtime.Internal.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Amazon.Runtime.Internal
{
	public sealed class UnityRequest : IHttpRequest<string>, IDisposable, IUnityHttpRequest
	{
		private bool _disposed;

		public Uri RequestUri
		{
			get;
			private set;
		}

		public IDisposable WwwRequest
		{
			get;
			set;
		}

		public byte[] RequestContent
		{
			get;
			private set;
		}

		public Dictionary<string, string> Headers
		{
			get;
			private set;
		}

		public AsyncCallback Callback
		{
			get;
			private set;
		}

		public IAsyncResult AsyncResult
		{
			get;
			private set;
		}

		public ManualResetEvent WaitHandle
		{
			get;
			private set;
		}

		public bool IsSync
		{
			get;
			set;
		}

		public IWebResponseData Response
		{
			get;
			set;
		}

		public Exception Exception
		{
			get;
			set;
		}

		public string Method
		{
			get;
			set;
		}

		private StreamReadTracker Tracker
		{
			get;
			set;
		}

		public UnityRequest(Uri requestUri)
		{
			RequestUri = requestUri;
			Headers = new Dictionary<string, string>();
		}

		public void ConfigureRequest(IRequestContext requestContext)
		{
		}

		public void SetRequestHeaders(IDictionary<string, string> headers)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				if (!header.Key.Equals("host", StringComparison.InvariantCultureIgnoreCase) && !header.Key.Equals("Content-Length", StringComparison.InvariantCultureIgnoreCase) && !header.Key.Equals("User-Agent", StringComparison.InvariantCultureIgnoreCase))
				{
					Headers.Add(header);
				}
			}
		}

		public string GetRequestContent()
		{
			return string.Empty;
		}

		public IWebResponseData GetResponse()
		{
			if (!UnityInitializer.IsMainThread())
			{
				IsSync = true;
				WaitHandle = new ManualResetEvent(initialState: false);
				try
				{
					UnityRequestQueue.Instance.EnqueueRequest(this);
					WaitHandle.WaitOne();
					if (Exception != null)
					{
						throw Exception;
					}
					if (Exception == null && Response == null)
					{
						throw new WebException("Request timed out", WebExceptionStatus.Timeout);
					}
					return Response;
				}
				finally
				{
					WaitHandle.Close();
				}
			}
			throw new Exception("Cannot execute synchronous calls on game thread");
		}

		public void WriteToRequestBody(string requestContent, Stream contentStream, IDictionary<string, string> contentHeaders, IRequestContext requestContext)
		{
			byte[] array = new byte[8192];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int count;
				while ((count = contentStream.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				RequestContent = memoryStream.ToArray();
			}
		}

		public void WriteToRequestBody(string requestContent, byte[] content, IDictionary<string, string> contentHeaders)
		{
			RequestContent = content;
		}

		public void Abort()
		{
		}

		public IAsyncResult BeginGetRequestContent(AsyncCallback callback, object state)
		{
			SimpleAsyncResult simpleAsyncResult = new SimpleAsyncResult(state);
			callback(simpleAsyncResult);
			return simpleAsyncResult;
		}

		public string EndGetRequestContent(IAsyncResult asyncResult)
		{
			return string.Empty;
		}

		public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			Callback = callback;
			AsyncResult = new SimpleAsyncResult(state);
			UnityRequestQueue.Instance.EnqueueRequest(this);
			return AsyncResult;
		}

		public IWebResponseData EndGetResponse(IAsyncResult asyncResult)
		{
			if (Exception != null)
			{
				throw Exception;
			}
			return Response;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
				{
					_disposed = true;
				});
			}
		}

		public Stream SetupProgressListeners(Stream originalStream, long progressUpdateInterval, object sender, EventHandler<StreamTransferProgressArgs> callback)
		{
			Tracker = new StreamReadTracker(sender, callback, originalStream.Length, progressUpdateInterval);
			return originalStream;
		}

		public void OnUploadProgressChanged(float progress)
		{
			if (Tracker != null)
			{
				Tracker.UpdateProgress(progress);
			}
		}
	}
}
