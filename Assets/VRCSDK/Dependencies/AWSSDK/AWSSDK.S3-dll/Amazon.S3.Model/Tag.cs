namespace Amazon.S3.Model
{
	public class Tag
	{
		private string key;

		private string value;

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

		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetValue()
		{
			return value != null;
		}
	}
}
