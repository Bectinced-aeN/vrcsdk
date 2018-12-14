using System;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class CopyObjectRequest : PutWithACLRequest
	{
		private string srcBucket;

		private string srcKey;

		private string srcVersionId;

		private string dstBucket;

		private string dstKey;

		private RequestPayer requestPayer;

		private S3CannedACL cannedACL;

		private string etagToMatch;

		private string etagToNotMatch;

		private DateTime? modifiedSinceDate;

		private DateTime? unmodifiedSinceDate;

		private List<Tag> tagset;

		private S3MetadataDirective metadataDirective;

		private S3StorageClass storageClass;

		private string websiteRedirectLocation;

		private HeadersCollection headersCollection = new HeadersCollection();

		private MetadataCollection metadataCollection = new MetadataCollection();

		private ServerSideEncryptionMethod serverSideEncryption;

		private ServerSideEncryptionCustomerMethod serverSideCustomerEncryption;

		private string serverSideEncryptionCustomerProvidedKey;

		private string serverSideEncryptionCustomerProvidedKeyMD5;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		private ServerSideEncryptionCustomerMethod copySourceServerSideCustomerEncryption;

		private string copySourceServerSideEncryptionCustomerProvidedKey;

		private string copySourceServerSideEncryptionCustomerProvidedKeyMD5;

		public string SourceBucket
		{
			get
			{
				return srcBucket;
			}
			set
			{
				srcBucket = value;
			}
		}

		public string SourceKey
		{
			get
			{
				return srcKey;
			}
			set
			{
				srcKey = value;
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

		public string DestinationBucket
		{
			get
			{
				return dstBucket;
			}
			set
			{
				dstBucket = value;
			}
		}

		public string DestinationKey
		{
			get
			{
				return dstKey;
			}
			set
			{
				dstKey = value;
			}
		}

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

		public string ETagToMatch
		{
			get
			{
				return etagToMatch;
			}
			set
			{
				etagToMatch = value;
			}
		}

		public string ETagToNotMatch
		{
			get
			{
				return etagToNotMatch;
			}
			set
			{
				etagToNotMatch = value;
			}
		}

		public DateTime ModifiedSinceDate
		{
			get
			{
				return modifiedSinceDate.GetValueOrDefault();
			}
			set
			{
				modifiedSinceDate = value;
			}
		}

		public DateTime UnmodifiedSinceDate
		{
			get
			{
				return unmodifiedSinceDate.GetValueOrDefault();
			}
			set
			{
				unmodifiedSinceDate = value;
			}
		}

		public S3MetadataDirective MetadataDirective
		{
			get
			{
				return metadataDirective;
			}
			set
			{
				metadataDirective = value;
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

		public ServerSideEncryptionCustomerMethod CopySourceServerSideEncryptionCustomerMethod
		{
			get
			{
				return copySourceServerSideCustomerEncryption;
			}
			set
			{
				copySourceServerSideCustomerEncryption = value;
			}
		}

		public string CopySourceServerSideEncryptionCustomerProvidedKey
		{
			get
			{
				return copySourceServerSideEncryptionCustomerProvidedKey;
			}
			set
			{
				copySourceServerSideEncryptionCustomerProvidedKey = value;
			}
		}

		public string CopySourceServerSideEncryptionCustomerProvidedKeyMD5
		{
			get
			{
				return copySourceServerSideEncryptionCustomerProvidedKeyMD5;
			}
			set
			{
				copySourceServerSideEncryptionCustomerProvidedKeyMD5 = value;
			}
		}

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

		internal bool IsSetSourceBucket()
		{
			return !string.IsNullOrEmpty(srcBucket);
		}

		internal bool IsSetSourceKey()
		{
			return !string.IsNullOrEmpty(srcKey);
		}

		internal bool IsSetSourceVersionId()
		{
			return !string.IsNullOrEmpty(srcVersionId);
		}

		internal bool IsSetDestinationBucket()
		{
			return !string.IsNullOrEmpty(dstBucket);
		}

		internal bool IsSetDestinationKey()
		{
			return !string.IsNullOrEmpty(dstKey);
		}

		internal bool IsSetCannedACL()
		{
			if (cannedACL != null)
			{
				return cannedACL != S3CannedACL.NoACL;
			}
			return false;
		}

		internal bool IsSetETagToMatch()
		{
			return !string.IsNullOrEmpty(etagToMatch);
		}

		internal bool IsSetETagToNotMatch()
		{
			return !string.IsNullOrEmpty(etagToNotMatch);
		}

		internal bool IsSetModifiedSinceDate()
		{
			return modifiedSinceDate.HasValue;
		}

		internal bool IsSetUnmodifiedSinceDate()
		{
			return unmodifiedSinceDate.HasValue;
		}

		internal bool IsSetServerSideEncryptionMethod()
		{
			if (serverSideEncryption != null)
			{
				return serverSideEncryption != ServerSideEncryptionMethod.None;
			}
			return false;
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

		internal bool IsSetCopySourceServerSideEncryptionCustomerMethod()
		{
			if (copySourceServerSideCustomerEncryption != null)
			{
				return copySourceServerSideCustomerEncryption != ServerSideEncryptionCustomerMethod.None;
			}
			return false;
		}

		internal bool IsSetCopySourceServerSideEncryptionCustomerProvidedKey()
		{
			return !string.IsNullOrEmpty(copySourceServerSideEncryptionCustomerProvidedKey);
		}

		internal bool IsSetCopySourceServerSideEncryptionCustomerProvidedKeyMD5()
		{
			return !string.IsNullOrEmpty(copySourceServerSideEncryptionCustomerProvidedKeyMD5);
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		internal bool IsSetTagSet()
		{
			return tagset != null;
		}
	}
}
