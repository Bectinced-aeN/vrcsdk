using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class RequestPayer : ConstantClass
	{
		public static readonly RequestPayer Requester = new RequestPayer("requester");

		private RequestPayer(string value)
			: this(value)
		{
		}

		public static RequestPayer FindValue(string value)
		{
			return ConstantClass.FindValue<RequestPayer>(value);
		}

		public static implicit operator RequestPayer(string value)
		{
			return ConstantClass.FindValue<RequestPayer>(value);
		}
	}
}
