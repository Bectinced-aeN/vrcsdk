namespace VRC.Core.BestHTTP.SocketIO.Events
{
	internal delegate void SocketIOCallback(Socket socket, Packet packet, params object[] args);
}
