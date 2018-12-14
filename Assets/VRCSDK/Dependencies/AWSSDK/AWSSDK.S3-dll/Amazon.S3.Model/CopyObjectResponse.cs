using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class CopyObjectResponse : AmazonWebServiceResponse
	{
		private string eTag;

		private string lastModified;

		private Expiration expiration;

		private string srcVersionId;

		private ServerSideEncryptionMethod serverSideEncryption;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		private RequestCharged requestCharged;

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

		public string LastModified
		{
			get
			{
				return lastModified;
			}
			set
			{
				lastModified = value;
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

		public string SourceVersionId
		{
			get
			{
				return srcVersionId;
			}
			set
			{
				srcVersionId = value;
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

		internal bool IsSetServerSideEncryptionKeyManagementServiceKeyId()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionKeyManagementServiceKeyId);
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		public CopyObjectResponse()
			: this()
		{
		}
	}
}
