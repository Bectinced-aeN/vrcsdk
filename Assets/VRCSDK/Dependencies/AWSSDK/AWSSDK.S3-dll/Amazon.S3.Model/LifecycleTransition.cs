using System;

namespace Amazon.S3.Model
{
	public class LifecycleTransition
	{
		private DateTime? date;

		private int? days;

		private S3StorageClass storageClass;

		public DateTime Date
		{
			get
			{
				return date ?? default(DateTime);
			}
			set
			{
				date = value;
			}
		}

		public int Days
		{
			get
			{
				return days ?? 0;
			}
			set
			{
				days = value;
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

		internal bool IsSetDate()
		{
			return date.HasValue;
		}

		internal bool IsSetDays()
		{
			return days.HasValue;
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}
	}
}
