using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketLoggingResponse : AmazonWebServiceResponse
	{
		private S3BucketLoggingConfig bucketLoggingConfig;

		public S3BucketLoggingConfig BucketLoggingConfig
		{
			get
			{
				if (bucketLoggingConfig == null)
				{
					bucketLoggingConfig = new S3BucketLoggingConfig();
				}
				return bucketLoggingConfig;
			}
			set
			{
				bucketLoggingConfig = value;
			}
		}

		public GetBucketLoggingResponse()
			: this()
		{
		}
	}
}
