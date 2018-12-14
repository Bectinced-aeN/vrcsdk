using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class ListObjectsByTagsRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private TagQuery tagQuery = new TagQuery();

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

		public TagQuery TagQuery
		{
			get
			{
				return tagQuery;
			}
			set
			{
				tagQuery = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		public ListObjectsByTagsRequest()
			: this()
		{
		}
	}
}
