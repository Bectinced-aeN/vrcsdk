using VRC.Core.BestHTTP.SignalR.Messages;

namespace VRC.Core.BestHTTP.SignalR.Hubs
{
	internal delegate void OnMethodProgressDelegate(Hub hub, ClientMessage originialMessage, ProgressMessage progress);
}
