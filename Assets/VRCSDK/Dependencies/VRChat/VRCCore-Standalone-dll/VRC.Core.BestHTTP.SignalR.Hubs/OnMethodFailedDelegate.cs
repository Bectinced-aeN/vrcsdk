using VRC.Core.BestHTTP.SignalR.Messages;

namespace VRC.Core.BestHTTP.SignalR.Hubs
{
	internal delegate void OnMethodFailedDelegate(Hub hub, ClientMessage originalMessage, FailureMessage error);
}
