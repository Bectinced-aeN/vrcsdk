using System;

namespace Amazon.Runtime.Internal
{
	public sealed class UnityWwwRequestFactory : IHttpRequestFactory<string>, IDisposable
	{
		private UnityWwwRequest _unityWwwRequest;

		public IHttpRequest<string> CreateHttpRequest(Uri requestUri)
		{
			_unityWwwRequest = new UnityWwwRequest(requestUri);
			return _unityWwwRequest;
		}

		public void Dispose()
		{
			if (_unityWwwRequest != null)
			{
				_unityWwwRequest.Dispose();
			}
		}
	}
}
