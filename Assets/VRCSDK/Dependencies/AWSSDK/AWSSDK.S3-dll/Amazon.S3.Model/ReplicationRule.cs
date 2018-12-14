namespace Amazon.S3.Model
{
	public class ReplicationRule
	{
		private string id;

		private string prefix;

		private ReplicationRuleStatus status;

		private ReplicationDestination destination;

		public string Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				prefix = value;
			}
		}

		public ReplicationRuleStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}

		public ReplicationDestination Destination
		{
			get
			{
				return destination;
			}
			set
			{
				destination = value;
			}
		}

		internal bool IsSetId()
		{
			return !string.IsNullOrEmpty(id);
		}

		internal bool IsSetPrefix()
		{
			return !string.IsNullOrEmpty(prefix);
		}

		internal bool IsSetStatus()
		{
			return status != null;
		}

		internal bool IsSetDestination()
		{
			return destination != null;
		}
	}
}
