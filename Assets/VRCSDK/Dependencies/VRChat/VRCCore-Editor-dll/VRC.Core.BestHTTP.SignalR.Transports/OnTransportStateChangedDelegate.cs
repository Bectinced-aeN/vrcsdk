namespace VRC.Core.BestHTTP.SignalR.Transports
{
	internal delegate void OnTransportStateChangedDelegate(TransportBase transport, TransportStates oldState, TransportStates newState);
}
