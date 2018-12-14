namespace Amazon.S3.Model
{
	public class AccelerateConfiguration
	{
		private BucketAccelerateStatus status;

		public BucketAccelerateStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}

		internal bool IsSetBucketAccelerateStatus()
		{
			return status != null;
		}
	}
}
