using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class InventoryOptionalField : ConstantClass
	{
		public static readonly InventoryOptionalField Size = new InventoryOptionalField("Size");

		public static readonly InventoryOptionalField LastModifiedDate = new InventoryOptionalField("LastModifiedDate");

		public static readonly InventoryOptionalField StorageClass = new InventoryOptionalField("StorageClass");

		public static readonly InventoryOptionalField ETag = new InventoryOptionalField("ETag");

		public static readonly InventoryOptionalField IsMultipartUploaded = new InventoryOptionalField("IsMultipartUploaded");

		public static readonly InventoryOptionalField ReplicationStatus = new InventoryOptionalField("ReplicationStatus");

		public InventoryOptionalField(string value)
			: this(value)
		{
		}

		public static InventoryOptionalField FindValue(string value)
		{
			return ConstantClass.FindValue<InventoryOptionalField>(value);
		}

		public static implicit operator InventoryOptionalField(string value)
		{
			return ConstantClass.FindValue<InventoryOptionalField>(value);
		}
	}
}
