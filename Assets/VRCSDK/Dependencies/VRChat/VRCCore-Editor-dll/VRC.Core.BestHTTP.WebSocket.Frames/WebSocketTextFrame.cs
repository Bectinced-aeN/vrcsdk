using System.Text;

namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	internal sealed class WebSocketTextFrame : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type => WebSocketFrameTypes.Text;

		public WebSocketTextFrame(string text)
			: base(Encoding.UTF8.GetBytes(text))
		{
		}
	}
}
