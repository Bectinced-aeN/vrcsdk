namespace Amazon.S3.Model
{
	public class InventorySchedule
	{
		private InventoryFrequency inventoryFrequency;

		public InventoryFrequency Frequency
		{
			get
			{
				return inventoryFrequency;
			}
			set
			{
				inventoryFrequency = value;
			}
		}

		internal bool IsFrequency()
		{
			return inventoryFrequency != null;
		}
	}
}
