using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class Tagging
	{
		private List<Tag> tagSet = new List<Tag>();

		public List<Tag> TagSet
		{
			get;
			set;
		}
	}
}
