namespace Amazon.S3.Model
{
	public class LifecycleRuleNoncurrentVersionTransition
	{
		private int? noncurrentDays;

		private S3StorageClass storageClass;

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

		public S3StorageClass StorageClass
		{
			get
			{
				return storageClass;
			}
			set
			{
				storageClass = value;
			}
		}

		internal bool IsSetNoncurrentDays()
		{
			return noncurrentDays.HasValue;
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}
	}
}
