using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutACLRequest : AmazonWebServiceRequest
	{
		private S3CannedACL cannedACL;

		private S3AccessControlList accessControlPolicy;

		private string bucket;

		private string key;

		private string versionId;

		public S3CannedACL CannedACL
		{
			get
			{
				return cannedACL;
			}
			set
			{
				cannedACL = value;
			}
		}

		public S3AccessControlList AccessControlList
		{
			get
			{
				return accessControlPolicy;
			}
			set
			{
				accessControlPolicy = value;
			}
		}

		public string BucketName
		{
			get
			{
				return bucket;
			}
			set
			{
				bucket = value;
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

		internal bool IsSetCannedACL()
		{
			return cannedACL != null;
		}

		internal bool IsSetAccessControlPolicy()
		{
			return accessControlPolicy != null;
		}

		internal bool IsSetBucketName()
		{
			return bucket != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
		}

		public PutACLRequest()
			: this()
		{
		}
	}
}
