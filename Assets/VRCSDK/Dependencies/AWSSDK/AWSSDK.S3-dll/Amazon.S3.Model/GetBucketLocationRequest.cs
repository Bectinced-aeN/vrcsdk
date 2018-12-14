using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketLocationRequest : AmazonWebServiceRequest
	{
		public string BucketName
		{
			get;
			set;
		}

		internal bool IsSetBucketName()
		{
			return BucketName != null;
		}

		public GetBucketLocationRequest()
			: this()
		{
		}
	}
}
