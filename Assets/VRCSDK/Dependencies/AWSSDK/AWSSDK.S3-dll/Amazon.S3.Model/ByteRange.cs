using System.Globalization;

namespace Amazon.S3.Model
{
	public class ByteRange
	{
		public long Start
		{
			get;
			set;
		}

		public long End
		{
			get;
			set;
		}

		public string FormattedByteRange => string.Format(CultureInfo.InvariantCulture, "bytes={0}-{1}", Start, End);

		public ByteRange(long start, long end)
		{
			Start = start;
			End = end;
		}
	}
}
