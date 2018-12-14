namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	internal sealed class WebSocketContinuationFrame : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type => WebSocketFrameTypes.Continuation;

		public WebSocketContinuationFrame(byte[] data, bool isFinal)
			: base(data, 0uL, (ulong)data.Length, isFinal)
		{
		}

		public WebSocketContinuationFrame(byte[] data, ulong pos, ulong length, bool isFinal)
			: base(data, pos, length, isFinal)
		{
		}
	}
}
