using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class VersionStatus : ConstantClass
	{
		public static readonly VersionStatus Off = new VersionStatus("Off");

		public static readonly VersionStatus Suspended = new VersionStatus("Suspended");

		public static readonly VersionStatus Enabled = new VersionStatus("Enabled");

		public VersionStatus(string value)
			: this(value)
		{
		}

		public static VersionStatus FindValue(string value)
		{
			return ConstantClass.FindValue<VersionStatus>(value);
		}

		public static implicit operator VersionStatus(string value)
		{
			return FindValue(value);
		}
	}
}
