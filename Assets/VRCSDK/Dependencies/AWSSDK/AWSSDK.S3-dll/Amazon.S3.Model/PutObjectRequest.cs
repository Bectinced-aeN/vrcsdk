using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

namespace Amazon.S3.Model
{
	public class PutObjectRequest : PutWithACLRequest
	{
		private S3CannedACL cannedACL;

		private string bucketName;

		private string key;

		private S3StorageClass storageClass;

		private string websiteRedirectLocation;

		private HeadersCollection headersCollection = new HeadersCollection();

		private MetadataCollection metadataCollection = new MetadataCollection();

		private ServerSideEncryptionMethod serverSideEncryption;

		private ServerSideEncryptionCustomerMethod serverSideCustomerEncryption;

		private string serverSideEncryptionCustomerProvidedKey;

		private string serverSideEncryptionCustomerProvidedKeyMD5;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		private Stream inputStream;

		private string filePath;

		private string contentBody;

		private bool autoCloseStream = true;

		private bool autoResetStreamPosition = true;

		private RequestPayer requestPayer;

		private string md5Digest;

		private List<Tag> tagset;

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

		public Stream InputStream
		{
			get
			{
				return inputStream;
			}
			set
			{
				inputStream = value;
			}
		}

		public string FilePath
		{
			get
			{
				return filePath;
			}
			set
			{
				filePath = value;
			}
		}

		public string ContentBody
		{
			get
			{
				return contentBody;
			}
			set
			{
				contentBody = value;
			}
		}

		public bool AutoCloseStream
		{
			get
			{
				return autoCloseStream;
			}
			set
			{
				autoCloseStream = value;
			}
		}

		public bool AutoResetStreamPosition
		{
			get
			{
				return autoResetStreamPosition;
			}
			set
			{
				autoResetStreamPosition = value;
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

		public string ServerSideEncryptionCustomerProvidedKey
		{
			get
			{
				return serverSideEncryptionCustomerProvidedKey;
			}
			set
			{
				serverSideEncryptionCustomerProvidedKey = value;
			}
		}

		public string ServerSideEncryptionCustomerProvidedKeyMD5
		{
			get
			{
				return serverSideEncryptionCustomerProvidedKeyMD5;
			}
			set
			{
				serverSideEncryptionCustomerProvidedKeyMD5 = value;
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

		public S3StorageClass StorageClass
		{
			get
			{
				return storageClass;
			}
			set
			{
				storageClass = value;
			}
		}

		public string WebsiteRedirectLocation
		{
			get
			{
				return websiteRedirectLocation;
			}
			set
			{
				websiteRedirectLocation = value;
			}
		}

		public EventHandler<StreamTransferProgressArgs> StreamTransferProgress
		{
			get
			{
				return this.get_StreamUploadProgressCallback();
			}
			set
			{
				this.set_StreamUploadProgressCallback(value);
			}
		}

		public string ContentType
		{
			get
			{
				return Headers.ContentType;
			}
			set
			{
				Headers.ContentType = value;
			}
		}

		public string MD5Digest
		{
			get
			{
				return md5Digest;
			}
			set
			{
				md5Digest = value;
			}
		}

		protected override bool IncludeSHA256Header => false;

		protected override bool Expect100Continue => true;

		public RequestPayer RequestPayer
		{
			get
			{
				return requestPayer;
			}
			set
			{
				requestPayer = value;
			}
		}

		public List<Tag> TagSet
		{
			get
			{
				return tagset;
			}
			set
			{
				tagset = value;
			}
		}

		internal bool IsSetCannedACL()
		{
			if (cannedACL != null)
			{
				return cannedACL != S3CannedACL.NoACL;
			}
			return false;
		}

		internal bool IsSetInputStream()
		{
			return inputStream != null;
		}

		internal bool IsSetBucket()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetServerSideEncryptionMethod()
		{
			if (serverSideEncryption != null)
			{
				return serverSideEncryption != ServerSideEncryptionMethod.None;
			}
			return false;
		}

		internal bool IsSetServerSideEncryptionCustomerMethod()
		{
			if (serverSideCustomerEncryption != null)
			{
				return serverSideCustomerEncryption != ServerSideEncryptionCustomerMethod.None;
			}
			return false;
		}

		internal bool IsSetServerSideEncryptionCustomerProvidedKey()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionCustomerProvidedKey);
		}

		internal bool IsSetServerSideEncryptionCustomerProvidedKeyMD5()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionCustomerProvidedKeyMD5);
		}

		internal bool IsSetServerSideEncryptionKeyManagementServiceKeyId()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionKeyManagementServiceKeyId);
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}

		internal bool IsSetWebsiteRedirectLocation()
		{
			return websiteRedirectLocation != null;
		}

		internal bool IsSetMD5Digest()
		{
			return !string.IsNullOrEmpty(md5Digest);
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		internal bool IsSetTagSet()
		{
			return tagset != null;
		}

		internal void SetupForFilePath()
		{
			FileStream fileStream = (FileStream)(InputStream = File.Open(FilePath, FileMode.Open));
			if (string.IsNullOrEmpty(Key))
			{
				Key = Path.GetFileName(FilePath);
			}
		}
	}
}
