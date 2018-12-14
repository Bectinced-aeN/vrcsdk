namespace VRC.Core.BestHTTP.SignalR.Messages
{
	internal sealed class KeepAliveMessage : IServerMessage
	{
		MessageTypes IServerMessage.Type
		{
			get
			{
				return MessageTypes.KeepAlive;
			}
		}

		void IServerMessage.Parse(object data)
		{
		}
	}
}
