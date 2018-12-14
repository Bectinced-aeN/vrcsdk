using System.Collections.Generic;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core.BestHTTP.SocketIO.JsonEncoders
{
	internal sealed class DefaultJSonEncoder : IJsonEncoder
	{
		public List<object> Decode(string json)
		{
			return Json.Decode(json) as List<object>;
		}

		public string Encode(List<object> obj)
		{
			return Json.Encode(obj);
		}
	}
}
