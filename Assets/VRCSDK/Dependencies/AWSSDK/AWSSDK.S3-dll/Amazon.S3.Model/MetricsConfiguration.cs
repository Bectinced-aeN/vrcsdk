namespace Amazon.S3.Model
{
	public class MetricsConfiguration
	{
		private string metricsId;

		private MetricsFilter metricsFilter;

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

		public MetricsFilter MetricsFilter
		{
			get
			{
				return metricsFilter;
			}
			set
			{
				metricsFilter = value;
			}
		}

		internal bool IsSetMetricsId()
		{
			return metricsId != null;
		}

		internal bool IsSetMetricsFilter()
		{
			return metricsFilter != null;
		}
	}
}
