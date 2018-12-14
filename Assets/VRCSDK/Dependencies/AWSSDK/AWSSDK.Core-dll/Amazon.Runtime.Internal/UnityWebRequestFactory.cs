using System;

namespace Amazon.Runtime.Internal
{
	public sealed class UnityWebRequestFactory : IHttpRequestFactory<string>, IDisposable
	{
		private UnityRequest _unityRequest;

		public IHttpRequest<string> CreateHttpRequest(Uri requestUri)
		{
			_unityRequest = new UnityRequest(requestUri);
			return _unityRequest;
		}

		public void Dispose()
		{
			if (_unityRequest != null)
			{
				_unityRequest.Dispose();
			}
		}
	}
}
