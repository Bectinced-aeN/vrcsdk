using System.Collections;
using System.Collections.Generic;

namespace VRC.Core.BestHTTP.SignalR.Messages
{
	internal sealed class MethodCallMessage : IServerMessage
	{
		MessageTypes IServerMessage.Type
		{
			get
			{
				return MessageTypes.MethodCall;
			}
		}

		public string Hub
		{
			get;
			private set;
		}

		public string Method
		{
			get;
			private set;
		}

		public object[] Arguments
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
			Hub = dictionary["H"].ToString();
			Method = dictionary["M"].ToString();
			List<object> list = new List<object>();
			foreach (object item in dictionary["A"] as IEnumerable)
			{
				list.Add(item);
			}
			Arguments = list.ToArray();
			if (dictionary.TryGetValue("S", out object value))
			{
				State = (value as IDictionary<string, object>);
			}
		}
	}
}
