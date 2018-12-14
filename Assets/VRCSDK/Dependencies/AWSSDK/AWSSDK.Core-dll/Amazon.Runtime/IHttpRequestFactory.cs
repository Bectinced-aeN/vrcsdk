using System;

namespace Amazon.Runtime
{
	public interface IHttpRequestFactory<TRequestContent> : IDisposable
	{
		IHttpRequest<TRequestContent> CreateHttpRequest(Uri requestUri);
	}
}
