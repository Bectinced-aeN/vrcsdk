namespace Amazon.S3.Model
{
	public class LifecycleRuleNoncurrentVersionExpiration
	{
		private int? noncurrentDays;

		public int NoncurrentDays
		{
			get
			{
				return noncurrentDays ?? 0;
			}
			set
			{
				noncurrentDays = value;
			}
		}

		internal bool IsSetNoncurrentDays()
		{
			return noncurrentDays.HasValue;
		}
	}
}
