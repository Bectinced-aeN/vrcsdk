using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketWebsiteRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		public string BucketName
		{
			get
			{
				return bucketName;
			}
			set
			{
				bucketName = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		public GetBucketWebsiteRequest()
			: this()
		{
		}
	}
}
