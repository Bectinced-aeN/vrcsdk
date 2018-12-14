using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class DeleteObjectTaggingRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

		private string versionId;

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

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetKey()
		{
			return !string.IsNullOrEmpty(key);
		}

		internal bool IsSetVersionId()
		{
			return !string.IsNullOrEmpty(versionId);
		}

		public DeleteObjectTaggingRequest()
			: this()
		{
		}
	}
}
