namespace VRC.Core.BestHTTP
{
	public enum HTTPRequestStates
	{
		Initial,
		Queued,
		Processing,
		Finished,
		Error,
		Aborted,
		ConnectionTimedOut,
		TimedOut
	}
}
