namespace VRC.Core.BestHTTP.SignalR.Messages
{
	internal sealed class DataMessage : IServerMessage
	{
		MessageTypes IServerMessage.Type
		{
			get
			{
				return MessageTypes.Data;
			}
		}

		public object Data
		{
			get;
			private set;
		}

		void IServerMessage.Parse(object data)
		{
			Data = data;
		}
	}
}
