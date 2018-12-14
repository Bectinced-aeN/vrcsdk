using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class BucketAccelerateStatus : ConstantClass
	{
		public static readonly BucketAccelerateStatus Enabled = new BucketAccelerateStatus("Enabled");

		public static readonly BucketAccelerateStatus Suspended = new BucketAccelerateStatus("Suspended");

		public BucketAccelerateStatus(string value)
			: this(value)
		{
		}

		public static BucketAccelerateStatus FindValue(string value)
		{
			return ConstantClass.FindValue<BucketAccelerateStatus>(value);
		}

		public static implicit operator BucketAccelerateStatus(string value)
		{
			return FindValue(value);
		}
	}
}
