namespace Amazon.S3.Model
{
	public class Filter
	{
		private S3KeyFilter s3KeyFilter;

		public S3KeyFilter S3KeyFilter
		{
			get
			{
				return s3KeyFilter;
			}
			set
			{
				s3KeyFilter = value;
			}
		}

		internal bool IsSetS3KeyFilter()
		{
			return s3KeyFilter != null;
		}
	}
}
