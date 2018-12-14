using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class DeleteBucketInventoryConfigurationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string inventoryId;

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

		public string InventoryId
		{
			get
			{
				return inventoryId;
			}
			set
			{
				inventoryId = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetInventoryId()
		{
			return !string.IsNullOrEmpty(inventoryId);
		}

		public DeleteBucketInventoryConfigurationRequest()
			: this()
		{
		}
	}
}
