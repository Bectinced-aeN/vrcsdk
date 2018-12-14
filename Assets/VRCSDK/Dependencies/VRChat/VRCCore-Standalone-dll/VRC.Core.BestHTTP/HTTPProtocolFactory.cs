using System;
using System.IO;
using VRC.Core.BestHTTP.ServerSentEvents;
using VRC.Core.BestHTTP.WebSocket;

namespace VRC.Core.BestHTTP
{
	internal static class HTTPProtocolFactory
	{
		public static HTTPResponse Get(SupportedProtocols protocol, HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
		{
			switch (protocol)
			{
			case SupportedProtocols.WebSocket:
				return new WebSocketResponse(request, stream, isStreamed, isFromCache);
			case SupportedProtocols.ServerSentEvents:
				return new EventSourceResponse(request, stream, isStreamed, isFromCache);
			default:
				return new HTTPResponse(request, stream, isStreamed, isFromCache);
			}
		}

		public static SupportedProtocols GetProtocolFromUri(Uri uri)
		{
			if (!(uri == null) && uri.Scheme != null)
			{
				string text = uri.Scheme.ToLowerInvariant();
				switch (text)
				{
				case "ws":
				case "wss":
					return SupportedProtocols.WebSocket;
				default:
					return SupportedProtocols.HTTP;
				}
			}
			throw new Exception("Malformed URI in GetProtocolFromUri");
		}

		public static bool IsSecureProtocol(Uri uri)
		{
			if (!(uri == null) && uri.Scheme != null)
			{
				string text = uri.Scheme.ToLowerInvariant();
				switch (text)
				{
				case "https":
				case "wss":
					return true;
				default:
					return false;
				}
			}
			throw new Exception("Malformed URI in IsSecureProtocol");
		}
	}
}
