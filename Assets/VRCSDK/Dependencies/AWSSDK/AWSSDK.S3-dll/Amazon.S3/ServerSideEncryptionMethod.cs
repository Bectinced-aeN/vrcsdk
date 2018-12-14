using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class ServerSideEncryptionMethod : ConstantClass
	{
		public static readonly ServerSideEncryptionMethod None = new ServerSideEncryptionMethod("");

		public static readonly ServerSideEncryptionMethod AES256 = new ServerSideEncryptionMethod("AES256");

		public static readonly ServerSideEncryptionMethod AWSKMS = new ServerSideEncryptionMethod("aws:kms");

		public ServerSideEncryptionMethod(string value)
			: this(value)
		{
		}

		public static ServerSideEncryptionMethod FindValue(string value)
		{
			return ConstantClass.FindValue<ServerSideEncryptionMethod>(value);
		}

		public static implicit operator ServerSideEncryptionMethod(string value)
		{
			return FindValue(value);
		}
	}
}
