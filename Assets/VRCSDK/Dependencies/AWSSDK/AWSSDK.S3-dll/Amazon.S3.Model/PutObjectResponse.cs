using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutObjectResponse : AmazonWebServiceResponse
	{
		private Expiration expiration;

		private ServerSideEncryptionMethod serverSideEncryption;

		private string eTag;

		private string versionId;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		private RequestCharged requestCharged;

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

		public PutObjectResponse()
			: this()
		{
		}
	}
}
