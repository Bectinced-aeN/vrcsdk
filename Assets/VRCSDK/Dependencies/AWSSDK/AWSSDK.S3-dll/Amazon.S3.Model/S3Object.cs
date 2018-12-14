using System;

namespace Amazon.S3.Model
{
	public class S3Object
	{
		private string eTag;

		private string key;

		private DateTime? lastModified;

		private Owner owner;

		private long? size;

		private S3StorageClass storageClass;

		private string bucketName;

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

		public long Size
		{
			get
			{
				return size ?? 0;
			}
			set
			{
				size = value;
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

		internal bool IsSetETag()
		{
			return eTag != null;
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetLastModified()
		{
			return lastModified.HasValue;
		}

		internal bool IsSetOwner()
		{
			return owner != null;
		}

		internal bool IsSetSize()
		{
			return size.HasValue;
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}
	}
}
