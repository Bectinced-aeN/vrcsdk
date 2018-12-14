using VRC.Core.BestHTTP.SignalR.Messages;

namespace VRC.Core.BestHTTP.SignalR.Hubs
{
	internal delegate void OnMethodResultDelegate(Hub hub, ClientMessage originalMessage, ResultMessage result);
}
