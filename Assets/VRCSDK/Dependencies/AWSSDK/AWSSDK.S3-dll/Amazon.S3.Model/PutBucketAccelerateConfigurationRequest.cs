using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketAccelerateConfigurationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private AccelerateConfiguration accelerateConfiguration;

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

		public AccelerateConfiguration AccelerateConfiguration
		{
			get
			{
				return accelerateConfiguration;
			}
			set
			{
				accelerateConfiguration = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return BucketName != null;
		}

		internal bool IsSetAccelerateConfiguration()
		{
			return AccelerateConfiguration != null;
		}

		public PutBucketAccelerateConfigurationRequest()
			: this()
		{
		}
	}
}
