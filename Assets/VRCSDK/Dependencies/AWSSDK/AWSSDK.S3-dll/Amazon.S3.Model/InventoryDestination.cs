namespace Amazon.S3.Model
{
	public class InventoryDestination
	{
		private InventoryS3BucketDestination inventoryS3BucketDestination;

		public InventoryS3BucketDestination S3BucketDestination
		{
			get
			{
				return inventoryS3BucketDestination;
			}
			set
			{
				inventoryS3BucketDestination = value;
			}
		}

		public bool isSetS3BucketDestination()
		{
			return inventoryS3BucketDestination != null;
		}
	}
}
