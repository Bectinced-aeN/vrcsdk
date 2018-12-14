using System.Collections.Generic;

namespace VRC.Core.BestHTTP.SignalR.Messages
{
	internal sealed class ResultMessage : IServerMessage, IHubMessage
	{
		MessageTypes IServerMessage.Type
		{
			get
			{
				return MessageTypes.Result;
			}
		}

		public ulong InvocationId
		{
			get;
			private set;
		}

		public object ReturnValue
		{
			get;
			private set;
		}

		public IDictionary<string, object> State
		{
			get;
			private set;
		}

		void IServerMessage.Parse(object data)
		{
			IDictionary<string, object> dictionary = data as IDictionary<string, object>;
			InvocationId = ulong.Parse(dictionary["I"].ToString());
			if (dictionary.TryGetValue("R", out object value))
			{
				ReturnValue = value;
			}
			if (dictionary.TryGetValue("S", out value))
			{
				State = (value as IDictionary<string, object>);
			}
		}
	}
}
