namespace Amazon.S3.Model
{
	public class KeyVersion
	{
		private string key;

		private string versionId;

		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public string VersionId
		{
			get
			{
				return versionId;
			}
			set
			{
				versionId = value;
			}
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
		}
	}
}
