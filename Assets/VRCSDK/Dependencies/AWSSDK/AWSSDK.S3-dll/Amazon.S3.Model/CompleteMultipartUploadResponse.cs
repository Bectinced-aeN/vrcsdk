using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class CompleteMultipartUploadResponse : AmazonWebServiceResponse
	{
		private string location;

		private string bucketName;

		private string key;

		private string eTag;

		private string versionId;

		private Expiration expiration;

		private ServerSideEncryptionMethod serverSideEncryption;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		private RequestCharged requestCharged;

		public string Location
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
			}
		}

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

		public string ETag
		{
			get
			{
				return eTag;
			}
			set
			{
				eTag = value;
			}
		}

		public Expiration Expiration
		{
			get
			{
				return expiration;
			}
			set
			{
				expiration = value;
			}
		}

		public ServerSideEncryptionMethod ServerSideEncryptionMethod
		{
			get
			{
				return serverSideEncryption;
			}
			set
			{
				serverSideEncryption = value;
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

		public string ServerSideEncryptionKeyManagementServiceKeyId
		{
			get
			{
				return serverSideEncryptionKeyManagementServiceKeyId;
			}
			set
			{
				serverSideEncryptionKeyManagementServiceKeyId = value;
			}
		}

		public RequestCharged RequestCharged
		{
			get
			{
				return requestCharged;
			}
			set
			{
				requestCharged = value;
			}
		}

		internal bool IsSetLocation()
		{
			return location != null;
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetETag()
		{
			return eTag != null;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
		}

		internal bool IsSetServerSideEncryptionKeyManagementServiceKeyId()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionKeyManagementServiceKeyId);
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		public CompleteMultipartUploadResponse()
			: this()
		{
		}
	}
}
