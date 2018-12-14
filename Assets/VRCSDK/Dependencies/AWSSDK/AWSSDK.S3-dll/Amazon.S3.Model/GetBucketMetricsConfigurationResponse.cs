using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketMetricsConfigurationResponse : AmazonWebServiceResponse
	{
		private MetricsConfiguration metricsConfiguration;

		public MetricsConfiguration MetricsConfiguration
		{
			get
			{
				return metricsConfiguration;
			}
			set
			{
				metricsConfiguration = value;
			}
		}

		internal bool IsSetMetricsConfiguration()
		{
			return metricsConfiguration != null;
		}

		public GetBucketMetricsConfigurationResponse()
			: this()
		{
		}
	}
}
