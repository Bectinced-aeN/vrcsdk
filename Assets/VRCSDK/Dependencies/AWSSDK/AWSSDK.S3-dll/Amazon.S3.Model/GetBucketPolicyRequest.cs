using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketPolicyRequest : AmazonWebServiceRequest
	{
		public string BucketName
		{
			get;
			set;
		}

		internal bool IsSetBucket()
		{
			return BucketName != null;
		}

		public GetBucketPolicyRequest()
			: this()
		{
		}
	}
}
