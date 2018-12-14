namespace VRC.Core.BestHTTP.ServerSentEvents
{
	internal delegate void OnStateChangedDelegate(EventSource eventSource, States oldState, States newState);
}
