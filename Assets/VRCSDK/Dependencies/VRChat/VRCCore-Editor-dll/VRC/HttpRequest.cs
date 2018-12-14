using VRC.Core.BestHTTP;

namespace VRC
{
	public class HttpRequest
	{
		private object InternalRequest;

		public HttpRequest(object req)
		{
			InternalRequest = req;
		}

		public void Abort()
		{
			if (InternalRequest != null)
			{
				(InternalRequest as HTTPRequest)?.Abort();
			}
		}
	}
}
