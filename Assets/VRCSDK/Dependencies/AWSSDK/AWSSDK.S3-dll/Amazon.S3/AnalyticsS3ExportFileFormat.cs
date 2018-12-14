using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class AnalyticsS3ExportFileFormat : ConstantClass
	{
		public static readonly AnalyticsS3ExportFileFormat CSV = new AnalyticsS3ExportFileFormat("CSV");

		public AnalyticsS3ExportFileFormat(string value)
			: this(value)
		{
		}

		public static AnalyticsS3ExportFileFormat FindValue(string value)
		{
			return ConstantClass.FindValue<AnalyticsS3ExportFileFormat>(value);
		}

		public static implicit operator AnalyticsS3ExportFileFormat(string value)
		{
			return ConstantClass.FindValue<AnalyticsS3ExportFileFormat>(value);
		}
	}
}
