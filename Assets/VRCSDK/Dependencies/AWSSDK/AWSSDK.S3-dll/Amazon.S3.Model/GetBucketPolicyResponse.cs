using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketPolicyResponse : AmazonWebServiceResponse
	{
		public string Policy
		{
			get;
			set;
		}

		internal bool IsSetPolicy()
		{
			return Policy != null;
		}

		public GetBucketPolicyResponse()
			: this()
		{
		}
	}
}
