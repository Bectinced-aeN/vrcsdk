using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class DeleteBucketMetricsConfigurationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string metricsId;

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

		public string MetricsId
		{
			get
			{
				return metricsId;
			}
			set
			{
				metricsId = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetMetricsId()
		{
			return !string.IsNullOrEmpty(MetricsId);
		}

		public DeleteBucketMetricsConfigurationRequest()
			: this()
		{
		}
	}
}
