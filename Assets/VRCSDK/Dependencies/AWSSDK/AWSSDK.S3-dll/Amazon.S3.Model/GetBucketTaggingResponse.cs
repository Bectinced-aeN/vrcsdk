using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class GetBucketTaggingResponse : AmazonWebServiceResponse
	{
		private List<Tag> tagSet = new List<Tag>();

		public List<Tag> TagSet
		{
			get
			{
				return tagSet;
			}
			set
			{
				tagSet = value;
			}
		}

		internal bool IsSetTagSet()
		{
			return tagSet.Count > 0;
		}

		public GetBucketTaggingResponse()
			: this()
		{
		}
	}
}
