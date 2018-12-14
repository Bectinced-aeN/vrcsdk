using Amazon.Runtime.Internal.Transform;

namespace Amazon.Runtime.Internal
{
	public class ResponseContext : IResponseContext
	{
		public AmazonWebServiceResponse Response
		{
			get;
			set;
		}

		public IWebResponseData HttpResponse
		{
			get;
			set;
		}
	}
}
