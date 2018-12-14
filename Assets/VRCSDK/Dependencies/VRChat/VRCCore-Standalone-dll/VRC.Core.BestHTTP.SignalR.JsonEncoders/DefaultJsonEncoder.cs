using System.Collections.Generic;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core.BestHTTP.SignalR.JsonEncoders
{
	internal sealed class DefaultJsonEncoder : IJsonEncoder
	{
		public string Encode(object obj)
		{
			return Json.Encode(obj);
		}

		public IDictionary<string, object> DecodeMessage(string json)
		{
			bool success = false;
			IDictionary<string, object> dictionary = Json.Decode(json, ref success) as IDictionary<string, object>;
			object result;
			if (success)
			{
				IDictionary<string, object> dictionary2 = dictionary;
				result = dictionary2;
			}
			else
			{
				result = null;
			}
			return (IDictionary<string, object>)result;
		}
	}
}
