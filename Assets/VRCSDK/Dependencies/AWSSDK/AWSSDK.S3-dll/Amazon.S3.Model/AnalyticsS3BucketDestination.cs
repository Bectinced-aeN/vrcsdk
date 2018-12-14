using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class AnalyticsS3BucketDestination
	{
		private AnalyticsS3ExportFileFormat analyticsS3ExportFileFormat;

		private string accountId;

		private string bucketName;

		private string prefix;

		public string Format
		{
			get
			{
				return ConstantClass.op_Implicit(analyticsS3ExportFileFormat);
			}
			set
			{
				analyticsS3ExportFileFormat = value;
			}
		}

		public string BucketAccountId
		{
			get
			{
				return accountId;
			}
			set
			{
				accountId = value;
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

		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				prefix = value;
			}
		}

		internal bool IsSetFormat()
		{
			return analyticsS3ExportFileFormat != null;
		}

		internal bool IsSetBucketAccountId()
		{
			return !string.IsNullOrEmpty(accountId);
		}

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetPrefix()
		{
			return !string.IsNullOrEmpty(prefix);
		}
	}
}
