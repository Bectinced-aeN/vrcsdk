namespace Amazon.S3.Model
{
	public class Initiator
	{
		private string displayName;

		private string iD;

		public string DisplayName
		{
			get
			{
				return displayName;
			}
			set
			{
				displayName = value;
			}
		}

		public string Id
		{
			get
			{
				return iD;
			}
			set
			{
				iD = value;
			}
		}

		internal bool IsSetDisplayName()
		{
			return displayName != null;
		}

		internal bool IsSetId()
		{
			return iD != null;
		}
	}
}
