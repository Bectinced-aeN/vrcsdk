using System;
using VRC.Core.BestHTTP.SignalR.Messages;
using VRC.Core.BestHTTP.WebSocket;

namespace VRC.Core.BestHTTP.SignalR.Transports
{
	internal sealed class WebSocketTransport : TransportBase
	{
		private VRC.Core.BestHTTP.WebSocket.WebSocket wSocket;

		public override bool SupportsKeepAlive => true;

		public override TransportTypes Type => TransportTypes.WebSocket;

		public WebSocketTransport(Connection connection)
			: base("webSockets", connection)
		{
		}

		public override void Connect()
		{
			if (wSocket != null)
			{
				HTTPManager.Logger.Warning("WebSocketTransport", "Start - WebSocket already created!");
			}
			else
			{
				if (base.State != TransportStates.Reconnecting)
				{
					base.State = TransportStates.Connecting;
				}
				RequestTypes type = (base.State != TransportStates.Reconnecting) ? RequestTypes.Connect : RequestTypes.Reconnect;
				Uri uri = base.Connection.BuildUri(type, this);
				wSocket = new VRC.Core.BestHTTP.WebSocket.WebSocket(uri);
				VRC.Core.BestHTTP.WebSocket.WebSocket webSocket = wSocket;
				webSocket.OnOpen = (OnWebSocketOpenDelegate)Delegate.Combine(webSocket.OnOpen, new OnWebSocketOpenDelegate(WSocket_OnOpen));
				VRC.Core.BestHTTP.WebSocket.WebSocket webSocket2 = wSocket;
				webSocket2.OnMessage = (OnWebSocketMessageDelegate)Delegate.Combine(webSocket2.OnMessage, new OnWebSocketMessageDelegate(WSocket_OnMessage));
				VRC.Core.BestHTTP.WebSocket.WebSocket webSocket3 = wSocket;
				webSocket3.OnClosed = (OnWebSocketClosedDelegate)Delegate.Combine(webSocket3.OnClosed, new OnWebSocketClosedDelegate(WSocket_OnClosed));
				VRC.Core.BestHTTP.WebSocket.WebSocket webSocket4 = wSocket;
				webSocket4.OnErrorDesc = (OnWebSocketErrorDescriptionDelegate)Delegate.Combine(webSocket4.OnErrorDesc, new OnWebSocketErrorDescriptionDelegate(WSocket_OnError));
				base.Connection.PrepareRequest(wSocket.InternalRequest, type);
				wSocket.Open();
			}
		}

		protected override void SendImpl(string json)
		{
			if (wSocket != null && wSocket.IsOpen)
			{
				wSocket.Send(json);
			}
		}

		public override void Stop()
		{
			if (wSocket != null)
			{
				wSocket.OnOpen = null;
				wSocket.OnMessage = null;
				wSocket.OnClosed = null;
				wSocket.OnErrorDesc = null;
				wSocket.Close();
				wSocket = null;
			}
		}

		protected override void Started()
		{
		}

		protected override void Aborted()
		{
			if (wSocket != null && wSocket.IsOpen)
			{
				wSocket.Close();
				wSocket = null;
			}
		}

		private void WSocket_OnOpen(VRC.Core.BestHTTP.WebSocket.WebSocket webSocket)
		{
			if (webSocket == wSocket)
			{
				HTTPManager.Logger.Information("WebSocketTransport", "WSocket_OnOpen");
				OnConnected();
			}
		}

		private void WSocket_OnMessage(VRC.Core.BestHTTP.WebSocket.WebSocket webSocket, string message)
		{
			if (webSocket == wSocket)
			{
				IServerMessage serverMessage = TransportBase.Parse(base.Connection.JsonEncoder, message);
				if (serverMessage != null)
				{
					base.Connection.OnMessage(serverMessage);
				}
			}
		}

		private void WSocket_OnClosed(VRC.Core.BestHTTP.WebSocket.WebSocket webSocket, ushort code, string message)
		{
			if (webSocket == wSocket)
			{
				string text = code.ToString() + " : " + message;
				HTTPManager.Logger.Information("WebSocketTransport", "WSocket_OnClosed " + text);
				if (base.State == TransportStates.Closing)
				{
					base.State = TransportStates.Closed;
				}
				else
				{
					base.Connection.Error(text);
				}
			}
		}

		private void WSocket_OnError(VRC.Core.BestHTTP.WebSocket.WebSocket webSocket, string reason)
		{
			if (webSocket == wSocket)
			{
				if (base.State == TransportStates.Closing || base.State == TransportStates.Closed)
				{
					AbortFinished();
				}
				else
				{
					HTTPManager.Logger.Error("WebSocketTransport", "WSocket_OnError " + reason);
					base.Connection.Error(reason);
				}
			}
		}
	}
}
