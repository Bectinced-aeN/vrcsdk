using System;

namespace Amazon.S3.Model
{
	public class PartETag : IComparable<PartETag>
	{
		private int? partNumber;

		private string eTag;

		public int PartNumber
		{
			get
			{
				return partNumber.GetValueOrDefault();
			}
			set
			{
				partNumber = value;
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

		public PartETag()
		{
		}

		public PartETag(int partNumber, string eTag)
		{
			this.partNumber = partNumber;
			this.eTag = eTag;
		}

		public int CompareTo(PartETag other)
		{
			return PartNumber.CompareTo(other.PartNumber);
		}

		internal bool IsSetPartNumber()
		{
			return partNumber.HasValue;
		}

		internal bool IsSetETag()
		{
			return eTag != null;
		}
	}
}
