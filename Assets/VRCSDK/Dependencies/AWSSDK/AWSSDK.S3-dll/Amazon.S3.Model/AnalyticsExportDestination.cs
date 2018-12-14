namespace Amazon.S3.Model
{
	public class AnalyticsExportDestination
	{
		private AnalyticsS3BucketDestination analyticsS3BucketDestination;

		public AnalyticsS3BucketDestination S3BucketDestination
		{
			get
			{
				return analyticsS3BucketDestination;
			}
			set
			{
				analyticsS3BucketDestination = value;
			}
		}

		internal bool IsSetS3BucketDestination()
		{
			return analyticsS3BucketDestination != null;
		}
	}
}
