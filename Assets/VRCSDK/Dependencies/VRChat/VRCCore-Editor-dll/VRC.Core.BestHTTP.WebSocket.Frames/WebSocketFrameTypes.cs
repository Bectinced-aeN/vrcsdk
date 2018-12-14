namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	public enum WebSocketFrameTypes : byte
	{
		Continuation = 0,
		Text = 1,
		Binary = 2,
		ConnectionClose = 8,
		Ping = 9,
		Pong = 10
	}
}
