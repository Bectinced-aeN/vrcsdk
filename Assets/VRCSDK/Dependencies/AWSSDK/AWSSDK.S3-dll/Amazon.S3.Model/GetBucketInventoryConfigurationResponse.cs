using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketInventoryConfigurationResponse : AmazonWebServiceResponse
	{
		private InventoryConfiguration inventoryConfiguration;

		public InventoryConfiguration InventoryConfiguration
		{
			get
			{
				return inventoryConfiguration;
			}
			set
			{
				inventoryConfiguration = value;
			}
		}

		internal bool IsSetInventoryConfiguration()
		{
			return inventoryConfiguration != null;
		}

		public GetBucketInventoryConfigurationResponse()
			: this()
		{
		}
	}
}
