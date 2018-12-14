namespace VRC.Core.BestHTTP.SignalR.Messages
{
	internal interface IServerMessage
	{
		MessageTypes Type
		{
			get;
		}

		void Parse(object data);
	}
}
