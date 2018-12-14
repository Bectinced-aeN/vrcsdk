namespace VRC.Core.BestHTTP
{
	internal enum HTTPConnectionStates
	{
		Initial,
		Processing,
		Redirected,
		Upgraded,
		WaitForProtocolShutdown,
		WaitForRecycle,
		Free,
		AbortRequested,
		TimedOut,
		Closed
	}
}
