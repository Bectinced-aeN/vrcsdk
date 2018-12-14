using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutCORSConfigurationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private CORSConfiguration configuration;

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

		public CORSConfiguration Configuration
		{
			get
			{
				return configuration;
			}
			set
			{
				configuration = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetConfiguration()
		{
			return configuration != null;
		}

		public PutCORSConfigurationRequest()
			: this()
		{
		}
	}
}
