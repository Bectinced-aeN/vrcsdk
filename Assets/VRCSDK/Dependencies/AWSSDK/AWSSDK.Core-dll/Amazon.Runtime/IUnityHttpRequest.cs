using Amazon.Runtime.Internal.Transform;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Amazon.Runtime
{
	public interface IUnityHttpRequest
	{
		byte[] RequestContent
		{
			get;
		}

		Dictionary<string, string> Headers
		{
			get;
		}

		AsyncCallback Callback
		{
			get;
		}

		IAsyncResult AsyncResult
		{
			get;
		}

		ManualResetEvent WaitHandle
		{
			get;
		}

		bool IsSync
		{
			get;
			set;
		}

		Exception Exception
		{
			get;
			set;
		}

		IDisposable WwwRequest
		{
			get;
			set;
		}

		IWebResponseData Response
		{
			get;
			set;
		}

		void OnUploadProgressChanged(float progress);
	}
}
