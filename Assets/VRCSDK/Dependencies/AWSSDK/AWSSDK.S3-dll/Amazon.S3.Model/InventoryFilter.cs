namespace Amazon.S3.Model
{
	public class InventoryFilter
	{
		private InventoryFilterPredicate inventoryFilterPredicate;

		public InventoryFilterPredicate InventoryFilterPredicate
		{
			get
			{
				return inventoryFilterPredicate;
			}
			set
			{
				inventoryFilterPredicate = value;
			}
		}
	}
}
