using System.Collections.Generic;

namespace VRC.Core.BestHTTP.SignalR.JsonEncoders
{
	internal interface IJsonEncoder
	{
		string Encode(object obj);

		IDictionary<string, object> DecodeMessage(string json);
	}
}
