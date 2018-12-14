using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketLoggingRequest : AmazonWebServiceRequest
	{
		public string BucketName
		{
			get;
			set;
		}

		public S3BucketLoggingConfig LoggingConfig
		{
			get;
			set;
		}

		internal bool IsSetBucketName()
		{
			return BucketName != null;
		}

		internal bool IsSetLoggingConfig()
		{
			return LoggingConfig != null;
		}

		public PutBucketLoggingRequest()
			: this()
		{
		}
	}
}
