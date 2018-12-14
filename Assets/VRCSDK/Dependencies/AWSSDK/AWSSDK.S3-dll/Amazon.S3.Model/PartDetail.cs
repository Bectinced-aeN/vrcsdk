using System;

namespace Amazon.S3.Model
{
	public class PartDetail : PartETag
	{
		private DateTime? lastModified;

		private long? size;

		public DateTime LastModified
		{
			get
			{
				return lastModified.GetValueOrDefault();
			}
			set
			{
				lastModified = value;
			}
		}

		public long Size
		{
			get
			{
				return size.GetValueOrDefault();
			}
			set
			{
				size = value;
			}
		}

		internal bool IsLastModified()
		{
			return lastModified.HasValue;
		}

		internal bool IsSize()
		{
			return size.HasValue;
		}
	}
}
