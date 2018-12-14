using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketAnalyticsConfigurationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string analyticsId;

		private AnalyticsConfiguration analyticsConfiguration;

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

		public string AnalyticsId
		{
			get
			{
				return analyticsId;
			}
			set
			{
				analyticsId = value;
			}
		}

		public AnalyticsConfiguration AnalyticsConfiguration
		{
			get
			{
				return analyticsConfiguration;
			}
			set
			{
				analyticsConfiguration = value;
			}
		}

		internal bool IsSetBucket()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetAnalyticsId()
		{
			return !string.IsNullOrEmpty(analyticsId);
		}

		internal bool IsSetAnalyticsConfiguration()
		{
			return analyticsConfiguration != null;
		}

		public PutBucketAnalyticsConfigurationRequest()
			: this()
		{
		}
	}
}
