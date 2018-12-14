using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class StorageClassAnalysisSchemaVersion : ConstantClass
	{
		public static readonly StorageClassAnalysisSchemaVersion V_1 = new StorageClassAnalysisSchemaVersion("V_1");

		public StorageClassAnalysisSchemaVersion(string value)
			: this(value)
		{
		}

		public static StorageClassAnalysisSchemaVersion FindValue(string value)
		{
			return ConstantClass.FindValue<StorageClassAnalysisSchemaVersion>(value);
		}

		public static implicit operator StorageClassAnalysisSchemaVersion(string value)
		{
			return ConstantClass.FindValue<StorageClassAnalysisSchemaVersion>(value);
		}
	}
}
