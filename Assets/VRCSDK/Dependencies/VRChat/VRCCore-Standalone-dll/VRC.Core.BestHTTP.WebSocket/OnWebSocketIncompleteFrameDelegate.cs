using VRC.Core.BestHTTP.WebSocket.Frames;

namespace VRC.Core.BestHTTP.WebSocket
{
	internal delegate void OnWebSocketIncompleteFrameDelegate(WebSocket webSocket, WebSocketFrameReader frame);
}
