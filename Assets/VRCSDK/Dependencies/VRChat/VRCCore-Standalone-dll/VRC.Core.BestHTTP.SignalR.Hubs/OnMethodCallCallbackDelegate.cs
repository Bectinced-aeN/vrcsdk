using VRC.Core.BestHTTP.SignalR.Messages;

namespace VRC.Core.BestHTTP.SignalR.Hubs
{
	internal delegate void OnMethodCallCallbackDelegate(Hub hub, MethodCallMessage methodCall);
}
