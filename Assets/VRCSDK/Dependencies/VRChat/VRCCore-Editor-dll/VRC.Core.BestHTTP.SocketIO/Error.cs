namespace VRC.Core.BestHTTP.SocketIO
{
	internal sealed class Error
	{
		public SocketIOErrors Code
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public Error(SocketIOErrors code, string msg)
		{
			Code = code;
			Message = msg;
		}

		public override string ToString()
		{
			return $"Code: {Code.ToString()} Message: \"{Message}\"";
		}
	}
}
