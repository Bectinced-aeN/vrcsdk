using Amazon.Runtime.Internal.Transform;
using System;
using System.Collections.Generic;
using System.IO;

namespace Amazon.Runtime
{
	public interface IHttpRequest<TRequestContent> : IDisposable
	{
		string Method
		{
			get;
			set;
		}

		Uri RequestUri
		{
			get;
		}

		void ConfigureRequest(IRequestContext requestContext);

		void SetRequestHeaders(IDictionary<string, string> headers);

		TRequestContent GetRequestContent();

		IWebResponseData GetResponse();

		void WriteToRequestBody(TRequestContent requestContent, Stream contentStream, IDictionary<string, string> contentHeaders, IRequestContext requestContext);

		void WriteToRequestBody(TRequestContent requestContent, byte[] content, IDictionary<string, string> contentHeaders);

		Stream SetupProgressListeners(Stream originalStream, long progressUpdateInterval, object sender, EventHandler<StreamTransferProgressArgs> callback);

		void Abort();

		IAsyncResult BeginGetRequestContent(AsyncCallback callback, object state);

		TRequestContent EndGetRequestContent(IAsyncResult asyncResult);

		IAsyncResult BeginGetResponse(AsyncCallback callback, object state);

		IWebResponseData EndGetResponse(IAsyncResult asyncResult);
	}
}
