namespace Amazon.S3.Model
{
	public class Owner
	{
		public string DisplayName
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		internal bool IsSetDisplayName()
		{
			return DisplayName != null;
		}

		internal bool IsSetId()
		{
			return Id != null;
		}
	}
}
