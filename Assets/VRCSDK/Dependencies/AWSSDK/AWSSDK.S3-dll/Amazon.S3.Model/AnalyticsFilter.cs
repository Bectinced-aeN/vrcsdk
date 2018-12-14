namespace Amazon.S3.Model
{
	public class AnalyticsFilter
	{
		private AnalyticsFilterPredicate analyticsFilterPredicate;

		public AnalyticsFilterPredicate AnalyticsFilterPredicate
		{
			get
			{
				return analyticsFilterPredicate;
			}
			set
			{
				analyticsFilterPredicate = value;
			}
		}
	}
}
