using Amazon.Runtime;
using System;

namespace Amazon.S3.Model
{
	public class InitiateMultipartUploadResponse : AmazonWebServiceResponse
	{
		private string bucketName;

		private string key;

		private string uploadId;

		private ServerSideEncryptionMethod serverSideEncryption;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		private DateTime? abortDate;

		private string abortRuleId;

		private RequestCharged requestCharged;

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

		public string UploadId
		{
			get
			{
				return uploadId;
			}
			set
			{
				uploadId = value;
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

		public DateTime AbortDate
		{
			get
			{
				return abortDate.GetValueOrDefault();
			}
			set
			{
				abortDate = value;
			}
		}

		public string AbortRuleId
		{
			get
			{
				return abortRuleId;
			}
			set
			{
				abortRuleId = value;
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

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetUploadId()
		{
			return uploadId != null;
		}

		internal bool IsSetServerSideEncryptionKeyManagementServiceKeyId()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionKeyManagementServiceKeyId);
		}

		internal bool IsSetAbortDate()
		{
			return abortDate.HasValue;
		}

		internal bool IsSetAbortRuleId()
		{
			return abortRuleId != null;
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		public InitiateMultipartUploadResponse()
			: this()
		{
		}
	}
}
