using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class PutBucketTaggingRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private List<Tag> tagSet = new List<Tag>();

		public string BucketName
		{
			get
			{
				return bucketName;
			}
			set
			{
				bucketName = value;
			}
		}

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

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetTagSet()
		{
			return tagSet.Count > 0;
		}

		public PutBucketTaggingRequest()
			: this()
		{
		}
	}
}
