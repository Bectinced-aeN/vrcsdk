using System;

namespace Amazon.S3.Model
{
	public class S3Bucket
	{
		private DateTime? creationDate;

		private string bucketName;

		public DateTime CreationDate
		{
			get
			{
				return creationDate ?? default(DateTime);
			}
			set
			{
				creationDate = value;
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

		internal bool IsSetCreationDate()
		{
			return creationDate.HasValue;
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}
	}
}
