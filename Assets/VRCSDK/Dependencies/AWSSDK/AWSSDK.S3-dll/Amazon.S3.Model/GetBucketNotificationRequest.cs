using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketNotificationRequest : AmazonWebServiceRequest
	{
		private string bucket;

		public string BucketName
		{
			get
			{
				return bucket;
			}
			set
			{
				bucket = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucket != null;
		}

		public GetBucketNotificationRequest()
			: this()
		{
		}
	}
}
