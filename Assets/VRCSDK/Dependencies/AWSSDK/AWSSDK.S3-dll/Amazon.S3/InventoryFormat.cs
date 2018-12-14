using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class InventoryFormat : ConstantClass
	{
		public static readonly InventoryFormat CSV = new InventoryFormat("CSV");

		public InventoryFormat(string value)
			: this(value)
		{
		}

		public static InventoryFormat FindValue(string value)
		{
			return ConstantClass.FindValue<InventoryFormat>(value);
		}

		public static implicit operator InventoryFormat(string value)
		{
			return ConstantClass.FindValue<InventoryFormat>(value);
		}
	}
}
