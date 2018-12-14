using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class TaggedResource
	{
		private string key;

		private List<Tag> tags = new List<Tag>();

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

		public List<Tag> Tags
		{
			get
			{
				return tags;
			}
			set
			{
				tags = value;
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
	}
}
