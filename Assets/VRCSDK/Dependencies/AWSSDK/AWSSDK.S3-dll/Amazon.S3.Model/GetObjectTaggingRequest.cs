using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetObjectTaggingRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

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

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetKey()
		{
			return !string.IsNullOrEmpty(key);
		}

		public GetObjectTaggingRequest()
			: this()
		{
		}
	}
}
