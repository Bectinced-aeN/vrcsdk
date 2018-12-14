using Amazon.Runtime;
using System;

namespace Amazon.S3.Model
{
	public class GetObjectMetadataRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private DateTime? modifiedSinceDate;

		private DateTime? unmodifiedSinceDate;

		private string etagToMatch;

		private string etagToNotMatch;

		private string key;

		private string versionId;

		private int? partNumber;

		private RequestPayer requestPayer;

		private ServerSideEncryptionCustomerMethod serverSideCustomerEncryption;

		private string serverSideEncryptionCustomerProvidedKey;

		private string serverSideEncryptionCustomerProvidedKeyMD5;

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

		public string EtagToMatch
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

		public DateTime ModifiedSinceDate
		{
			get
			{
				return modifiedSinceDate ?? default(DateTime);
			}
			set
			{
				modifiedSinceDate = value;
			}
		}

		public string EtagToNotMatch
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

		public DateTime UnmodifiedSinceDate
		{
			get
			{
				return unmodifiedSinceDate ?? default(DateTime);
			}
			set
			{
				unmodifiedSinceDate = value;
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

		public int? PartNumber
		{
			get
			{
				return partNumber;
			}
			set
			{
				if (value.HasValue && (value < 1 || 10000 < value))
				{
					throw new ArgumentException("PartNumber must be a positve integer between 1 and 10,000.");
				}
				partNumber = value;
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

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetEtagToMatch()
		{
			return etagToMatch != null;
		}

		internal bool IsSetModifiedSinceDate()
		{
			return modifiedSinceDate.HasValue;
		}

		internal bool IsSetEtagToNotMatch()
		{
			return etagToNotMatch != null;
		}

		internal bool IsSetUnmodifiedSinceDate()
		{
			return unmodifiedSinceDate.HasValue;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
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

		internal bool IsSetPartNumber()
		{
			return partNumber.HasValue;
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		public GetObjectMetadataRequest()
			: this()
		{
		}
	}
}
