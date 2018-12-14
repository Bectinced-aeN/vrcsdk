namespace Amazon.S3.Model
{
	public class ReplicationDestination
	{
		private string bucketArn;

		private S3StorageClass storageClass;

		public string BucketArn
		{
			get
			{
				return bucketArn;
			}
			set
			{
				bucketArn = value;
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

		internal bool IsSetBucketArn()
		{
			return !string.IsNullOrEmpty(bucketArn);
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}
	}
}
