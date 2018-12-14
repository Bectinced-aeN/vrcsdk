namespace VRC.Core.BestHTTP.SocketIO
{
	public enum TransportEventTypes
	{
		Unknown = -1,
		Open,
		Close,
		Ping,
		Pong,
		Message,
		Upgrade,
		Noop
	}
}
