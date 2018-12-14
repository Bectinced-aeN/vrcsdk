using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class ServerSideEncryptionCustomerMethod : ConstantClass
	{
		public static readonly ServerSideEncryptionCustomerMethod None = new ServerSideEncryptionCustomerMethod("");

		public static readonly ServerSideEncryptionCustomerMethod AES256 = new ServerSideEncryptionCustomerMethod("AES256");

		public ServerSideEncryptionCustomerMethod(string value)
			: this(value)
		{
		}

		public static ServerSideEncryptionCustomerMethod FindValue(string value)
		{
			return ConstantClass.FindValue<ServerSideEncryptionCustomerMethod>(value);
		}

		public static implicit operator ServerSideEncryptionCustomerMethod(string value)
		{
			return FindValue(value);
		}
	}
}
