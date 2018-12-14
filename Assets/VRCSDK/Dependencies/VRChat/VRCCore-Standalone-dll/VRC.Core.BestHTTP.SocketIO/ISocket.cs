namespace VRC.Core.BestHTTP.SocketIO
{
	internal interface ISocket
	{
		void Open();

		void Disconnect(bool remove);

		void OnPacket(Packet packet);

		void EmitEvent(SocketIOEventTypes type, params object[] args);

		void EmitEvent(string eventName, params object[] args);

		void EmitError(SocketIOErrors errCode, string msg);
	}
}
