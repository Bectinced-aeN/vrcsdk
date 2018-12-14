namespace Amazon.S3.Model
{
	public class InventoryS3BucketDestination
	{
		private string accountId;

		private string bucketName;

		private string prefix;

		private InventoryFormat inventoryFormat;

		public string AccountId
		{
			get
			{
				return accountId;
			}
			set
			{
				accountId = value;
			}
		}

		public string BucketName
		{
			get
			{
				return bucketName;
			}
			set
			{
				bucketName = value;
			}
		}

		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				prefix = value;
			}
		}

		public InventoryFormat InventoryFormat
		{
			get
			{
				return inventoryFormat;
			}
			set
			{
				inventoryFormat = value;
			}
		}

		public bool IsSetAccountId()
		{
			return !string.IsNullOrEmpty(accountId);
		}

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetPrefix()
		{
			return !string.IsNullOrEmpty(prefix);
		}

		internal bool IsSetInventoryFormat()
		{
			return inventoryFormat != null;
		}
	}
}
