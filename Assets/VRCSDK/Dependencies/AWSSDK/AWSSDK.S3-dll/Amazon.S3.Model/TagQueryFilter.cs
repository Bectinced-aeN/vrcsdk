using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class TagQueryFilter
	{
		private string key;

		private List<string> or;

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

		public List<string> Or
		{
			get
			{
				return or;
			}
			set
			{
				or = value;
			}
		}
	}
}
