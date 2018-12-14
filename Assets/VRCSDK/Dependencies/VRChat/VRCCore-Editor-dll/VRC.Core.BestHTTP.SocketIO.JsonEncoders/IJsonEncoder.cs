using System.Collections.Generic;

namespace VRC.Core.BestHTTP.SocketIO.JsonEncoders
{
	internal interface IJsonEncoder
	{
		List<object> Decode(string json);

		string Encode(List<object> obj);
	}
}
