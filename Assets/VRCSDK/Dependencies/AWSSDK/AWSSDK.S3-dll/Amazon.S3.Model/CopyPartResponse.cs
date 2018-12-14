using Amazon.Runtime;
using System;

namespace Amazon.S3.Model
{
	public class CopyPartResponse : AmazonWebServiceResponse
	{
		private DateTime? lastModified;

		private string eTag;

		private string copySourceVersionId;

		private int partNumber;

		private ServerSideEncryptionMethod serverSideEncryption;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		public string CopySourceVersionId
		{
			get
			{
				return copySourceVersionId;
			}
			set
			{
				copySourceVersionId = value;
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

		public DateTime LastModified
		{
			get
			{
				return lastModified ?? default(DateTime);
			}
			set
			{
				lastModified = value;
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

		public int PartNumber
		{
			get
			{
				return partNumber;
			}
			set
			{
				partNumber = value;
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

		internal bool IsSetCopySourceVersionId()
		{
			return copySourceVersionId != null;
		}

		internal bool IsSetETag()
		{
			return eTag != null;
		}

		internal bool IsSetLastModified()
		{
			return lastModified.HasValue;
		}

		internal bool IsSetServerSideEncryptionMethod()
		{
			return serverSideEncryption != null;
		}

		internal bool IsSetServerSideEncryptionKeyManagementServiceKeyId()
		{
			return !string.IsNullOrEmpty(serverSideEncryptionKeyManagementServiceKeyId);
		}

		public CopyPartResponse()
			: this()
		{
		}
	}
}
