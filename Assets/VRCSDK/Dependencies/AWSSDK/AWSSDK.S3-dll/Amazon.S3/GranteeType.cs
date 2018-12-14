using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class GranteeType : ConstantClass
	{
		public static readonly GranteeType Group = new GranteeType("Group");

		public static readonly GranteeType Email = new GranteeType("AmazonCustomerByEmail");

		public static readonly GranteeType CanonicalUser = new GranteeType("CanonicalUser");

		public GranteeType(string value)
			: this(value)
		{
		}

		public static GranteeType FindValue(string value)
		{
			return ConstantClass.FindValue<GranteeType>(value);
		}

		public static implicit operator GranteeType(string value)
		{
			return FindValue(value);
		}
	}
}
