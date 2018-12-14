using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketMetricsConfigurationRequest : AmazonWebServiceRequest
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
			return bucketName != null;
		}

		internal bool IsSetMetricsId()
		{
			return metricsId != null;
		}

		public GetBucketMetricsConfigurationRequest()
			: this()
		{
		}
	}
}
