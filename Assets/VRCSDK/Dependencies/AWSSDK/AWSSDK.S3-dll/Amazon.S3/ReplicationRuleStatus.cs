using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class ReplicationRuleStatus : ConstantClass
	{
		public static readonly ReplicationRuleStatus Enabled = new ReplicationRuleStatus("Enabled");

		public static readonly ReplicationRuleStatus Disabled = new ReplicationRuleStatus("Disabled");

		public ReplicationRuleStatus(string value)
			: this(value)
		{
		}

		public static ReplicationRuleStatus FindValue(string value)
		{
			return ConstantClass.FindValue<ReplicationRuleStatus>(value);
		}

		public static implicit operator ReplicationRuleStatus(string value)
		{
			return ConstantClass.FindValue<ReplicationRuleStatus>(value);
		}
	}
}
