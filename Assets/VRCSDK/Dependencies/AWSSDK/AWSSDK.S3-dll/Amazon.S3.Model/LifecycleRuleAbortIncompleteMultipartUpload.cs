namespace Amazon.S3.Model
{
	public class LifecycleRuleAbortIncompleteMultipartUpload
	{
		private int? daysAfterInitiation;

		public int DaysAfterInitiation
		{
			get
			{
				return daysAfterInitiation ?? 0;
			}
			set
			{
				daysAfterInitiation = value;
			}
		}

		internal bool IsSetDaysAfterInitiation()
		{
			return daysAfterInitiation.HasValue;
		}
	}
}
