using Amazon.Runtime;
using System;

namespace Amazon.S3.Model
{
	public class GetPreSignedUrlRequest : AmazonWebServiceRequest
	{
		private ResponseHeaderOverrides _responseHeaders;

		private string bucketName;

		private string key;

		private DateTime? expires;

		private Protocol protocol;

		private HttpVerb verb;

		private string versionId;

		private ServerSideEncryptionMethod encryption;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		private HeadersCollection headersCollection = new HeadersCollection();

		private MetadataCollection metadataCollection = new MetadataCollection();

		private ServerSideEncryptionCustomerMethod serverSideCustomerEncryption;

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

		public string ContentType
		{
			get
			{
				return headersCollection.ContentType;
			}
			set
			{
				headersCollection.ContentType = value;
			}
		}

		public DateTime Expires
		{
			get
			{
				return expires.GetValueOrDefault();
			}
			set
			{
				expires = value;
			}
		}

		public Protocol Protocol
		{
			get
			{
				return protocol;
			}
			set
			{
				protocol = value;
			}
		}

		public HttpVerb Verb
		{
			get
			{
				return verb;
			}
			set
			{
				verb = value;
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

		public ServerSideEncryptionMethod ServerSideEncryptionMethod
		{
			get
			{
				return encryption;
			}
			set
			{
				encryption = value;
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

		public ServerSideEncryptionCustomerMethod ServerSideEncryptionCustomerMethod
		{
			get
			{
				return serverSideCustomerEncryption;
			}
			set
			{
				serverSideCustomerEncryption = value;
			}
		}

		public ResponseHeaderOverrides ResponseHeaderOverrides
		{
			get
			{
				if (_responseHeaders == null)
				{
					_responseHeaders = new ResponseHeaderOverrides();
				}
				return _responseHeaders;
			}
			set
			{
				_responseHeaders = value;
			}
		}

		public HeadersCollection Headers
		{
			get
			{
				if (headersCollection == null)
				{
					headersCollection = new HeadersCollection();
				}
				return headersCollection;
			}
			internal set
			{
				headersCollection = value;
			}
		}

		public MetadataCollection Metadata
		{
			get
			{
				if (metadataCollection == null)
				{
					metadataCollection = new MetadataCollection();
				}
				return metadataCollection;
			}
			internal set
			{
				metadataCollection = value;
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

		public bool IsSetExpires()
		{
			return expires.HasValue;
		}

		internal bool IsSetVersionId()
		{
			return !string.IsNullOrEmpty(versionId);
		}

		internal bool IsSetServerSideEncryptionKeyManagementServiceKeyId()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionKeyManagementServiceKeyId);
		}

		internal bool IsSetServerSideEncryptionCustomerMethod()
		{
			if (serverSideCustomerEncryption != null)
			{
				return serverSideCustomerEncryption != ServerSideEncryptionCustomerMethod.None;
			}
			return false;
		}

		public GetPreSignedUrlRequest()
			: this()
		{
		}
	}
}
