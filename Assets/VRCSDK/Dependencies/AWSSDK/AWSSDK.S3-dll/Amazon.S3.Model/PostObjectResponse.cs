using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PostObjectResponse : AmazonWebServiceResponse
	{
		public string RequestId
		{
			get;
			set;
		}

		public string HostId
		{
			get;
			set;
		}

		public string VersionId
		{
			get;
			set;
		}

		public PostObjectResponse()
			: this()
		{
		}
	}
}
