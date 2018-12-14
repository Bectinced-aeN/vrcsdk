using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketAnalyticsConfigurationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string analyticsId;

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

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetAnalyticsId()
		{
			return !string.IsNullOrEmpty(analyticsId);
		}

		public GetBucketAnalyticsConfigurationRequest()
			: this()
		{
		}
	}
}
