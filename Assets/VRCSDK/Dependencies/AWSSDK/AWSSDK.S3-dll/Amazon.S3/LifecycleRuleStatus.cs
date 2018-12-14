using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class LifecycleRuleStatus : ConstantClass
	{
		public static readonly LifecycleRuleStatus Enabled = new LifecycleRuleStatus("Enabled");

		public static readonly LifecycleRuleStatus Disabled = new LifecycleRuleStatus("Disabled");

		public LifecycleRuleStatus(string value)
			: this(value)
		{
		}

		public static LifecycleRuleStatus FindValue(string value)
		{
			return ConstantClass.FindValue<LifecycleRuleStatus>(value);
		}

		public static implicit operator LifecycleRuleStatus(string value)
		{
			return FindValue(value);
		}
	}
}
