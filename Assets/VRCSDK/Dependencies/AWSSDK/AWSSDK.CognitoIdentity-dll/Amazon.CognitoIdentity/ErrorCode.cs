using Amazon.Runtime;

namespace Amazon.CognitoIdentity
{
	public class ErrorCode : ConstantClass
	{
		public static readonly ErrorCode AccessDenied = new ErrorCode("AccessDenied");

		public static readonly ErrorCode InternalServerError = new ErrorCode("InternalServerError");

		public ErrorCode(string value)
			: base(value)
		{
		}

		public static ErrorCode FindValue(string value)
		{
			return ConstantClass.FindValue<ErrorCode>(value);
		}

		public static implicit operator ErrorCode(string value)
		{
			return FindValue(value);
		}
	}
}
