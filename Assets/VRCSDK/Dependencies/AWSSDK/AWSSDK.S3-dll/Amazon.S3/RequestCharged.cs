using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class RequestCharged : ConstantClass
	{
		public static readonly RequestCharged Requester = new RequestCharged("requester");

		private RequestCharged(string value)
			: this(value)
		{
		}

		public static RequestCharged FindValue(string value)
		{
			return ConstantClass.FindValue<RequestCharged>(value);
		}

		public static implicit operator RequestCharged(string value)
		{
			return ConstantClass.FindValue<RequestCharged>(value);
		}
	}
}
