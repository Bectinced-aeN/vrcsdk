using System;

namespace Amazon.S3.Model
{
	public class MultipartUpload
	{
		private string key;

		private string uploadId;

		private Owner owner;

		private Initiator initiator;

		private DateTime? initiated;

		private S3StorageClass storageClass;

		public DateTime Initiated
		{
			get
			{
				return initiated ?? default(DateTime);
			}
			set
			{
				initiated = value;
			}
		}

		public Initiator Initiator
		{
			get
			{
				return initiator;
			}
			set
			{
				initiator = value;
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

		public Owner Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
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

		internal bool IsSetInitiated()
		{
			return initiated.HasValue;
		}

		internal bool IsSetInitiator()
		{
			return initiator != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetOwner()
		{
			return owner != null;
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}

		internal bool IsSetUploadId()
		{
			return uploadId != null;
		}
	}
}
