using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutObjectTaggingRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

		private Tagging tagging = new Tagging();

		private string md5Digest;

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

		public Tagging Tagging
		{
			get
			{
				return tagging;
			}
			set
			{
				tagging = value;
			}
		}

		internal bool IsSetBucket()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetTagging()
		{
			return tagging != null;
		}

		public PutObjectTaggingRequest()
			: this()
		{
		}
	}
}
