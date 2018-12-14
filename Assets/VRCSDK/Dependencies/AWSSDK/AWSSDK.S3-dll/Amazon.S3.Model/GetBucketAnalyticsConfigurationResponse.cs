using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketAnalyticsConfigurationResponse : AmazonWebServiceResponse
	{
		private AnalyticsConfiguration analyticsConfiguration;

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

		internal bool IsSetAnalyticsConfiguration()
		{
			return analyticsConfiguration != null;
		}

		public GetBucketAnalyticsConfigurationResponse()
			: this()
		{
		}
	}
}
