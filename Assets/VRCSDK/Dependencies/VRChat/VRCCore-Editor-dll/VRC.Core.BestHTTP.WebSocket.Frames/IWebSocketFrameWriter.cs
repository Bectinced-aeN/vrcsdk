namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	internal interface IWebSocketFrameWriter
	{
		WebSocketFrameTypes Type
		{
			get;
		}

		byte[] Get();
	}
}
