using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class InventoryFrequency : ConstantClass
	{
		public static readonly InventoryFrequency Daily = new InventoryFrequency("Daily");

		public static readonly InventoryFrequency Weekly = new InventoryFrequency("Weekly");

		public InventoryFrequency(string value)
			: this(value)
		{
		}

		public static InventoryFrequency FindValue(string value)
		{
			return ConstantClass.FindValue<InventoryFrequency>(value);
		}

		public static implicit operator InventoryFrequency(string value)
		{
			return ConstantClass.FindValue<InventoryFrequency>(value);
		}
	}
}
