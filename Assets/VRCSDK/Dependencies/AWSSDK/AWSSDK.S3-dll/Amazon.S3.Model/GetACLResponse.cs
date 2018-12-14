using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetACLResponse : AmazonWebServiceResponse
	{
		public S3AccessControlList AccessControlList
		{
			get;
			set;
		}

		public GetACLResponse()
			: this()
		{
		}
	}
}
