using VRC.Core.BestHTTP.SocketIO.Transports;

namespace VRC.Core.BestHTTP.SocketIO
{
	internal interface IManager
	{
		void Remove(Socket socket);

		void Close(bool removeSockets = true);

		void TryToReconnect();

		bool OnTransportConnected(ITransport transport);

		void OnTransportError(ITransport trans, string err);

		void SendPacket(Packet packet);

		void OnPacket(Packet packet);

		void EmitEvent(string eventName, params object[] args);

		void EmitEvent(SocketIOEventTypes type, params object[] args);

		void EmitError(SocketIOErrors errCode, string msg);

		void EmitAll(string eventName, params object[] args);
	}
}
