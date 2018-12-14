using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class ReplicationStatus : ConstantClass
	{
		public static readonly ReplicationStatus Pending = new ReplicationStatus("PENDING");

		public static readonly ReplicationStatus Completed = new ReplicationStatus("COMPLETED");

		public static readonly ReplicationStatus Replica = new ReplicationStatus("REPLICA");

		public static readonly ReplicationStatus Failed = new ReplicationStatus("FAILED");

		public ReplicationStatus(string value)
			: this(value)
		{
		}

		public static ReplicationStatus FindValue(string value)
		{
			return ConstantClass.FindValue<ReplicationStatus>(value);
		}

		public static implicit operator ReplicationStatus(string value)
		{
			return ConstantClass.FindValue<ReplicationStatus>(value);
		}
	}
}
