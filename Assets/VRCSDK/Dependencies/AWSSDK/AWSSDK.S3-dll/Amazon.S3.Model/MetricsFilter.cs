namespace Amazon.S3.Model
{
	public class MetricsFilter
	{
		private MetricsFilterPredicate metricsFilterPredicate;

		public MetricsFilterPredicate MetricsFilterPredicate
		{
			get
			{
				return metricsFilterPredicate;
			}
			set
			{
				metricsFilterPredicate = value;
			}
		}
	}
}
