using LitJson;
using System.Collections.Generic;

namespace VRC.Core.BestHTTP.SocketIO.JsonEncoders
{
	internal sealed class LitJsonEncoder : IJsonEncoder
	{
		public List<object> Decode(string json)
		{
			JsonReader reader = new JsonReader(json);
			return JsonMapper.ToObject<List<object>>(reader);
		}

		public string Encode(List<object> obj)
		{
			JsonWriter jsonWriter = new JsonWriter();
			JsonMapper.ToJson(obj, jsonWriter);
			return jsonWriter.ToString();
		}
	}
}
