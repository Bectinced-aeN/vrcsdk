using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutLifecycleConfigurationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private LifecycleConfiguration lifecycleConfiguration;

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

		public LifecycleConfiguration Configuration
		{
			get
			{
				return lifecycleConfiguration;
			}
			set
			{
				lifecycleConfiguration = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetConfiguration()
		{
			return lifecycleConfiguration != null;
		}

		public PutLifecycleConfigurationRequest()
			: this()
		{
		}
	}
}
