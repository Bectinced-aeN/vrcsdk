using System.Text;

namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	internal sealed class WebSocketPing : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type => WebSocketFrameTypes.Ping;

		public WebSocketPing(string msg)
			: base(Encoding.UTF8.GetBytes(msg))
		{
		}
	}
}
