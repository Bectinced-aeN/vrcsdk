using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class S3StorageClass : ConstantClass
	{
		public static readonly S3StorageClass Standard = new S3StorageClass("STANDARD");

		public static readonly S3StorageClass ReducedRedundancy = new S3StorageClass("REDUCED_REDUNDANCY");

		public static readonly S3StorageClass Glacier = new S3StorageClass("GLACIER");

		public static readonly S3StorageClass StandardInfrequentAccess = new S3StorageClass("STANDARD_IA");

		public S3StorageClass(string value)
			: this(value)
		{
		}

		public static S3StorageClass FindValue(string value)
		{
			return ConstantClass.FindValue<S3StorageClass>(value);
		}

		public static implicit operator S3StorageClass(string value)
		{
			return FindValue(value);
		}
	}
}
