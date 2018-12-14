using System;
using System.Collections.Generic;
using VRC.Core.BestHTTP.Logger;
using VRC.Core.BestHTTP.WebSocket;

namespace VRC.Core.BestHTTP.SocketIO.Transports
{
	internal sealed class WebSocketTransport : ITransport
	{
		private Packet PacketWithAttachment;

		private byte[] Buffer;

		public TransportStates State
		{
			get;
			private set;
		}

		public SocketManager Manager
		{
			get;
			private set;
		}

		public bool IsRequestInProgress
		{
			get;
			private set;
		}

		public VRC.Core.BestHTTP.WebSocket.WebSocket Implementation
		{
			get;
			private set;
		}

		public WebSocketTransport(SocketManager manager)
		{
			State = TransportStates.Closed;
			Manager = manager;
		}

		public void Open()
		{
			if (State == TransportStates.Closed)
			{
				Uri uri = new Uri(string.Format("{0}?transport=websocket&sid={1}{2}", new UriBuilder((!HTTPProtocolFactory.IsSecureProtocol(Manager.Uri)) ? "ws" : "wss", Manager.Uri.Host, Manager.Uri.Port, Manager.Uri.PathAndQuery).Uri.ToString(), Manager.Handshake.Sid, Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams()));
				Implementation = new VRC.Core.BestHTTP.WebSocket.WebSocket(uri);
				Implementation.OnOpen = OnOpen;
				Implementation.OnMessage = OnMessage;
				Implementation.OnBinary = OnBinary;
				Implementation.OnError = OnError;
				Implementation.OnClosed = OnClosed;
				Implementation.Open();
				State = TransportStates.Connecting;
			}
		}

		public void Close()
		{
			if (State != TransportStates.Closed)
			{
				State = TransportStates.Closed;
				if (Implementation != null)
				{
					Implementation.Close();
				}
				else
				{
					HTTPManager.Logger.Warning("WebSocketTransport", "Close - WebSocket Implementation already null!");
				}
				Implementation = null;
			}
		}

		public void Poll()
		{
		}

		private void OnOpen(VRC.Core.BestHTTP.WebSocket.WebSocket ws)
		{
			if (ws == Implementation)
			{
				HTTPManager.Logger.Information("WebSocketTransport", "OnOpen");
				State = TransportStates.Opening;
				Send(new Packet(TransportEventTypes.Ping, SocketIOEventTypes.Unknown, "/", "probe"));
			}
		}

		private void OnMessage(VRC.Core.BestHTTP.WebSocket.WebSocket ws, string message)
		{
			if (ws == Implementation)
			{
				if (HTTPManager.Logger.Level <= Loglevels.All)
				{
					HTTPManager.Logger.Verbose("WebSocketTransport", "OnMessage: " + message);
				}
				try
				{
					Packet packet = new Packet(message);
					if (packet.AttachmentCount == 0)
					{
						OnPacket(packet);
					}
					else
					{
						PacketWithAttachment = packet;
					}
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("WebSocketTransport", "OnMessage", ex);
				}
			}
		}

		private void OnBinary(VRC.Core.BestHTTP.WebSocket.WebSocket ws, byte[] data)
		{
			if (ws == Implementation)
			{
				if (HTTPManager.Logger.Level <= Loglevels.All)
				{
					HTTPManager.Logger.Verbose("WebSocketTransport", "OnBinary");
				}
				if (PacketWithAttachment != null)
				{
					PacketWithAttachment.AddAttachmentFromServer(data, copyFull: false);
					if (PacketWithAttachment.HasAllAttachment)
					{
						try
						{
							OnPacket(PacketWithAttachment);
						}
						catch (Exception ex)
						{
							HTTPManager.Logger.Exception("WebSocketTransport", "OnBinary", ex);
						}
						finally
						{
							PacketWithAttachment = null;
						}
					}
				}
			}
		}

		private void OnError(VRC.Core.BestHTTP.WebSocket.WebSocket ws, Exception ex)
		{
			if (ws == Implementation)
			{
				string text = string.Empty;
				if (ex == null)
				{
					switch (ws.InternalRequest.State)
					{
					case HTTPRequestStates.Finished:
						text = ((!ws.InternalRequest.Response.IsSuccess && ws.InternalRequest.Response.StatusCode != 101) ? $"Request Finished Successfully, but the server sent an error. Status Code: {ws.InternalRequest.Response.StatusCode}-{ws.InternalRequest.Response.Message} Message: {ws.InternalRequest.Response.DataAsText}" : $"Request finished. Status Code: {ws.InternalRequest.Response.StatusCode.ToString()} Message: {ws.InternalRequest.Response.Message}");
						break;
					case HTTPRequestStates.Error:
						text = (("Request Finished with Error! : " + ws.InternalRequest.Exception == null) ? string.Empty : (ws.InternalRequest.Exception.Message + " " + ws.InternalRequest.Exception.StackTrace));
						break;
					case HTTPRequestStates.Aborted:
						text = "Request Aborted!";
						break;
					case HTTPRequestStates.ConnectionTimedOut:
						text = "Connection Timed Out!";
						break;
					case HTTPRequestStates.TimedOut:
						text = "Processing the request Timed Out!";
						break;
					}
				}
				else
				{
					text = ex.Message + " " + ex.StackTrace;
				}
				HTTPManager.Logger.Error("WebSocketTransport", "OnError: " + text);
				((IManager)Manager).OnTransportError((ITransport)this, text);
			}
		}

		private void OnClosed(VRC.Core.BestHTTP.WebSocket.WebSocket ws, ushort code, string message)
		{
			if (ws == Implementation)
			{
				HTTPManager.Logger.Information("WebSocketTransport", "OnClosed");
				Close();
				((IManager)Manager).TryToReconnect();
			}
		}

		public void Send(Packet packet)
		{
			if (State != TransportStates.Closed && State != TransportStates.Paused)
			{
				string text = packet.Encode();
				if (HTTPManager.Logger.Level <= Loglevels.All)
				{
					HTTPManager.Logger.Verbose("WebSocketTransport", "Send: " + text);
				}
				if (packet.AttachmentCount != 0 || (packet.Attachments != null && packet.Attachments.Count != 0))
				{
					if (packet.Attachments == null)
					{
						throw new ArgumentException("packet.Attachments are null!");
					}
					if (packet.AttachmentCount != packet.Attachments.Count)
					{
						throw new ArgumentException("packet.AttachmentCount != packet.Attachments.Count. Use the packet.AddAttachment function to add data to a packet!");
					}
				}
				Implementation.Send(text);
				if (packet.AttachmentCount != 0)
				{
					int num = packet.Attachments[0].Length + 1;
					for (int i = 1; i < packet.Attachments.Count; i++)
					{
						if (packet.Attachments[i].Length + 1 > num)
						{
							num = packet.Attachments[i].Length + 1;
						}
					}
					if (Buffer == null || Buffer.Length < num)
					{
						Array.Resize(ref Buffer, num);
					}
					for (int j = 0; j < packet.AttachmentCount; j++)
					{
						Buffer[0] = 4;
						Array.Copy(packet.Attachments[j], 0, Buffer, 1, packet.Attachments[j].Length);
						Implementation.Send(Buffer, 0uL, (ulong)((long)packet.Attachments[j].Length + 1L));
					}
				}
			}
		}

		public void Send(List<Packet> packets)
		{
			for (int i = 0; i < packets.Count; i++)
			{
				Send(packets[i]);
			}
			packets.Clear();
		}

		private void OnPacket(Packet packet)
		{
			switch (packet.TransportEvent)
			{
			case TransportEventTypes.Message:
			case TransportEventTypes.Noop:
				if (State == TransportStates.Opening)
				{
					State = TransportStates.Open;
					if (!((IManager)Manager).OnTransportConnected((ITransport)this))
					{
						return;
					}
				}
				break;
			case TransportEventTypes.Pong:
				if (packet.Payload == "probe")
				{
					HTTPManager.Logger.Information("WebSocketTransport", "\"probe\" packet received, sending Upgrade packet");
					Send(new Packet(TransportEventTypes.Upgrade, SocketIOEventTypes.Unknown, "/", string.Empty));
				}
				break;
			}
			((IManager)Manager).OnPacket(packet);
		}
	}
}
