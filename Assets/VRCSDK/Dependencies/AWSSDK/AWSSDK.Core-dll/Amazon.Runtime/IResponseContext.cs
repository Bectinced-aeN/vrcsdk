using Amazon.Runtime.Internal.Transform;

namespace Amazon.Runtime
{
	public interface IResponseContext
	{
		AmazonWebServiceResponse Response
		{
			get;
			set;
		}

		IWebResponseData HttpResponse
		{
			get;
			set;
		}
	}
}
