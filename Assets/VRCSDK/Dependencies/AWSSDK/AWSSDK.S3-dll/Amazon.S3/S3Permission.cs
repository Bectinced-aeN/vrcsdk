using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class S3Permission : ConstantClass
	{
		public static readonly S3Permission READ = new S3Permission("READ", "x-amz-grant-read");

		public static readonly S3Permission WRITE = new S3Permission("WRITE", "x-amz-grant-write");

		public static readonly S3Permission READ_ACP = new S3Permission("READ_ACP", "x-amz-grant-read-acp");

		public static readonly S3Permission WRITE_ACP = new S3Permission("WRITE_ACP", "x-amz-grant-write-acp");

		public static readonly S3Permission FULL_CONTROL = new S3Permission("FULL_CONTROL", "x-amz-grant-full-control");

		public static readonly S3Permission RESTORE_OBJECT = new S3Permission("RESTORE", "x-amz-grant-restore-object");

		public string HeaderName
		{
			get;
			private set;
		}

		public S3Permission(string value)
			: this(value, null)
		{
		}

		public S3Permission(string value, string headerName)
			: this(value)
		{
			HeaderName = headerName;
		}

		public static S3Permission FindValue(string value)
		{
			return ConstantClass.FindValue<S3Permission>(value);
		}

		public static implicit operator S3Permission(string value)
		{
			return FindValue(value);
		}
	}
}
