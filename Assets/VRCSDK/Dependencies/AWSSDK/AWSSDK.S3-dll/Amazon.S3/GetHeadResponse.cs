using System.Net;

namespace Amazon.S3
{
	internal class GetHeadResponse
	{
		public HttpStatusCode? StatusCode
		{
			get;
			set;
		}

		public string HeaderValue
		{
			get;
			set;
		}
	}
}
