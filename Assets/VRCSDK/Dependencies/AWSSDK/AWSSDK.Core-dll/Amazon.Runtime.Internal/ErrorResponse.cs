namespace Amazon.Runtime.Internal
{
	public class ErrorResponse
	{
		private ErrorType type;

		private string code;

		private string message;

		private string requestId;

		public ErrorType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public string Code
		{
			get
			{
				return code;
			}
			set
			{
				code = value;
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
			set
			{
				message = value;
			}
		}

		public string RequestId
		{
			get
			{
				return requestId;
			}
			set
			{
				requestId = value;
			}
		}
	}
}
