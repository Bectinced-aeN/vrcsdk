namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	internal sealed class WebSocketPong : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type => WebSocketFrameTypes.Pong;

		public WebSocketPong(WebSocketFrameReader ping)
			: base(ping.Data)
		{
		}
	}
}
