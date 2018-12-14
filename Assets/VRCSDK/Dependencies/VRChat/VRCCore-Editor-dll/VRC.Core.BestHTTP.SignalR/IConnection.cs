using System;
using VRC.Core.BestHTTP.SignalR.JsonEncoders;
using VRC.Core.BestHTTP.SignalR.Messages;
using VRC.Core.BestHTTP.SignalR.Transports;

namespace VRC.Core.BestHTTP.SignalR
{
	internal interface IConnection
	{
		ProtocolVersions Protocol
		{
			get;
		}

		NegotiationData NegotiationResult
		{
			get;
		}

		IJsonEncoder JsonEncoder
		{
			get;
			set;
		}

		void OnMessage(IServerMessage msg);

		void TransportStarted();

		void TransportReconnected();

		void TransportAborted();

		void Error(string reason);

		Uri BuildUri(RequestTypes type);

		Uri BuildUri(RequestTypes type, TransportBase transport);

		HTTPRequest PrepareRequest(HTTPRequest req, RequestTypes type);

		string ParseResponse(string responseStr);
	}
}
