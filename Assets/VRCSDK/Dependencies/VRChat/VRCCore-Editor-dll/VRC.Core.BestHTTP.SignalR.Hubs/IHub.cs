using VRC.Core.BestHTTP.SignalR.Messages;

namespace VRC.Core.BestHTTP.SignalR.Hubs
{
	internal interface IHub
	{
		Connection Connection
		{
			get;
			set;
		}

		bool Call(ClientMessage msg);

		bool HasSentMessageId(ulong id);

		void Close();

		void OnMethod(MethodCallMessage msg);

		void OnMessage(IServerMessage msg);
	}
}
