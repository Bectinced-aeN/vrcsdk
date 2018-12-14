using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class InventoryIncludedObjectVersions : ConstantClass
	{
		public static readonly InventoryIncludedObjectVersions All = new InventoryIncludedObjectVersions("All");

		public static readonly InventoryIncludedObjectVersions Current = new InventoryIncludedObjectVersions("Current");

		public InventoryIncludedObjectVersions(string value)
			: this(value)
		{
		}

		public static InventoryIncludedObjectVersions FindValue(string value)
		{
			return ConstantClass.FindValue<InventoryIncludedObjectVersions>(value);
		}

		public static implicit operator InventoryIncludedObjectVersions(string value)
		{
			return ConstantClass.FindValue<InventoryIncludedObjectVersions>(value);
		}
	}
}
