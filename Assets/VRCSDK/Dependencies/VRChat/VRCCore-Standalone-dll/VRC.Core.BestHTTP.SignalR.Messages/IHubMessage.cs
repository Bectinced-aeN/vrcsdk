namespace VRC.Core.BestHTTP.SignalR.Messages
{
	internal interface IHubMessage
	{
		ulong InvocationId
		{
			get;
		}
	}
}
