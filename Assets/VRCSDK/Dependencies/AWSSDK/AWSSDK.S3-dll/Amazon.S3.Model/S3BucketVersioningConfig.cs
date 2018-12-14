namespace Amazon.S3.Model
{
	public class S3BucketVersioningConfig
	{
		private VersionStatus status = "Off";

		private bool? enableMfaDelete;

		public VersionStatus Status
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

		public bool EnableMfaDelete
		{
			get
			{
				return enableMfaDelete.GetValueOrDefault();
			}
			set
			{
				enableMfaDelete = value;
			}
		}

		internal bool IsSetStatus()
		{
			if (status != null)
			{
				return status != VersionStatus.Off;
			}
			return false;
		}

		internal bool IsSetEnableMfaDelete()
		{
			return enableMfaDelete.HasValue;
		}
	}
}
