namespace VRC.Core.BestHTTP.ServerSentEvents
{
	public enum States
	{
		Initial,
		Connecting,
		Open,
		Retrying,
		Closing,
		Closed
	}
}
