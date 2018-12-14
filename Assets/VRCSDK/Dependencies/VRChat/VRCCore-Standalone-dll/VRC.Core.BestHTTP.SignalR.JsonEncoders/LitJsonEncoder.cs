using LitJson;
using System.Collections.Generic;

namespace VRC.Core.BestHTTP.SignalR.JsonEncoders
{
	internal sealed class LitJsonEncoder : IJsonEncoder
	{
		public string Encode(object obj)
		{
			JsonWriter jsonWriter = new JsonWriter();
			JsonMapper.ToJson(obj, jsonWriter);
			return jsonWriter.ToString();
		}

		public IDictionary<string, object> DecodeMessage(string json)
		{
			JsonReader reader = new JsonReader(json);
			return JsonMapper.ToObject<Dictionary<string, object>>(reader);
		}
	}
}
