using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class S3CannedACL : ConstantClass
	{
		public static readonly S3CannedACL NoACL = new S3CannedACL("NoACL");

		public static readonly S3CannedACL Private = new S3CannedACL("private");

		public static readonly S3CannedACL PublicRead = new S3CannedACL("public-read");

		public static readonly S3CannedACL PublicReadWrite = new S3CannedACL("public-read-write");

		public static readonly S3CannedACL AuthenticatedRead = new S3CannedACL("authenticated-read");

		public static readonly S3CannedACL AWSExecRead = new S3CannedACL("aws-exec-read");

		public static readonly S3CannedACL BucketOwnerRead = new S3CannedACL("bucket-owner-read");

		public static readonly S3CannedACL BucketOwnerFullControl = new S3CannedACL("bucket-owner-full-control");

		public static readonly S3CannedACL LogDeliveryWrite = new S3CannedACL("log-delivery-write");

		public S3CannedACL(string value)
			: this(value)
		{
		}

		public static S3CannedACL FindValue(string value)
		{
			return ConstantClass.FindValue<S3CannedACL>(value);
		}

		public static implicit operator S3CannedACL(string value)
		{
			return FindValue(value);
		}
	}
}
