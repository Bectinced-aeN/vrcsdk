using System;
using System.Collections.Generic;
using VRC.Core.BestHTTP.Logger;

namespace VRC.Core.BestHTTP.SocketIO.Transports
{
	internal sealed class PollingTransport : ITransport
	{
		private HTTPRequest LastRequest;

		private HTTPRequest PollRequest;

		private Packet PacketWithAttachment;

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

		public bool IsRequestInProgress => LastRequest != null;

		public PollingTransport(SocketManager manager)
		{
			Manager = manager;
		}

		public void Open()
		{
			HTTPRequest hTTPRequest = new HTTPRequest(new Uri($"{Manager.Uri.ToString()}?EIO={4}&transport=polling&t={Manager.Timestamp.ToString()}-{Manager.RequestCounter++.ToString()}&sid={Manager.Handshake.Sid}{(Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())}&b64=true"), OnRequestFinished);
			hTTPRequest.DisableCache = true;
			hTTPRequest.DisableRetry = true;
			hTTPRequest.Send();
			State = TransportStates.Opening;
		}

		public void Close()
		{
			if (State != TransportStates.Closed)
			{
				State = TransportStates.Closed;
			}
		}

		public void Send(Packet packet)
		{
			Send(new List<Packet>
			{
				packet
			});
		}

		public void Send(List<Packet> packets)
		{
			if (State != TransportStates.Open)
			{
				throw new Exception("Transport is not in Open state!");
			}
			if (IsRequestInProgress)
			{
				throw new Exception("Sending packets are still in progress!");
			}
			byte[] array = null;
			try
			{
				array = packets[0].EncodeBinary();
				for (int i = 1; i < packets.Count; i++)
				{
					byte[] array2 = packets[i].EncodeBinary();
					Array.Resize(ref array, array.Length + array2.Length);
					Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
				}
				packets.Clear();
			}
			catch (Exception ex)
			{
				((IManager)Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
				return;
				IL_00b3:;
			}
			LastRequest = new HTTPRequest(new Uri($"{Manager.Uri.ToString()}?EIO={4}&transport=polling&t={Manager.Timestamp.ToString()}-{Manager.RequestCounter++.ToString()}&sid={Manager.Handshake.Sid}{(Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())}&b64=true"), HTTPMethods.Post, OnRequestFinished);
			LastRequest.DisableCache = true;
			LastRequest.SetHeader("Content-Type", "application/octet-stream");
			LastRequest.RawData = array;
			LastRequest.Send();
		}

		private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			LastRequest = null;
			if (State != TransportStates.Closed)
			{
				string text = null;
				switch (req.State)
				{
				case HTTPRequestStates.Finished:
					if (HTTPManager.Logger.Level <= Loglevels.All)
					{
						HTTPManager.Logger.Verbose("PollingTransport", "OnRequestFinished: " + resp.DataAsText);
					}
					if (resp.IsSuccess)
					{
						ParseResponse(resp);
					}
					else
					{
						text = $"Polling - Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} Uri: {req.CurrentUri}";
					}
					break;
				case HTTPRequestStates.Error:
					text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
					break;
				case HTTPRequestStates.Aborted:
					text = $"Polling - Request({req.CurrentUri}) Aborted!";
					break;
				case HTTPRequestStates.ConnectionTimedOut:
					text = $"Polling - Connection Timed Out! Uri: {req.CurrentUri}";
					break;
				case HTTPRequestStates.TimedOut:
					text = $"Polling - Processing the request({req.CurrentUri}) Timed Out!";
					break;
				}
				if (!string.IsNullOrEmpty(text))
				{
					((IManager)Manager).OnTransportError((ITransport)this, text);
				}
			}
		}

		public void Poll()
		{
			if (PollRequest == null && State != TransportStates.Paused)
			{
				PollRequest = new HTTPRequest(new Uri($"{Manager.Uri.ToString()}?EIO={4}&transport=polling&t={Manager.Timestamp.ToString()}-{Manager.RequestCounter++.ToString()}&sid={Manager.Handshake.Sid}{(Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())}&b64=true"), HTTPMethods.Get, OnPollRequestFinished);
				PollRequest.DisableCache = true;
				PollRequest.DisableRetry = true;
				PollRequest.Send();
			}
		}

		private void OnPollRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			PollRequest = null;
			if (State != TransportStates.Closed)
			{
				string text = null;
				switch (req.State)
				{
				case HTTPRequestStates.Finished:
					if (HTTPManager.Logger.Level <= Loglevels.All)
					{
						HTTPManager.Logger.Verbose("PollingTransport", "OnPollRequestFinished: " + resp.DataAsText);
					}
					if (resp.IsSuccess)
					{
						ParseResponse(resp);
					}
					else
					{
						text = $"Polling - Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} Uri: {req.CurrentUri}";
					}
					break;
				case HTTPRequestStates.Error:
					text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
					break;
				case HTTPRequestStates.Aborted:
					text = $"Polling - Request({req.CurrentUri}) Aborted!";
					break;
				case HTTPRequestStates.ConnectionTimedOut:
					text = $"Polling - Connection Timed Out! Uri: {req.CurrentUri}";
					break;
				case HTTPRequestStates.TimedOut:
					text = $"Polling - Processing the request({req.CurrentUri}) Timed Out!";
					break;
				}
				if (!string.IsNullOrEmpty(text))
				{
					((IManager)Manager).OnTransportError((ITransport)this, text);
				}
			}
		}

		private void OnPacket(Packet packet)
		{
			if (packet.AttachmentCount != 0 && !packet.HasAllAttachment)
			{
				PacketWithAttachment = packet;
			}
			else
			{
				TransportEventTypes transportEvent = packet.TransportEvent;
				if (transportEvent == TransportEventTypes.Message && packet.SocketIOEvent == SocketIOEventTypes.Connect && State == TransportStates.Opening)
				{
					State = TransportStates.Open;
					if (!((IManager)Manager).OnTransportConnected((ITransport)this))
					{
						return;
					}
				}
				((IManager)Manager).OnPacket(packet);
			}
		}

		private void ParseResponse(HTTPResponse resp)
		{
			try
			{
				if (resp != null && resp.Data != null && resp.Data.Length >= 1)
				{
					string dataAsText = resp.DataAsText;
					if (!(dataAsText == "ok"))
					{
						int num = dataAsText.IndexOf(':', 0);
						int num2 = 0;
						while (num >= 0 && num < dataAsText.Length)
						{
							int num3 = int.Parse(dataAsText.Substring(num2, num - num2));
							string text = dataAsText.Substring(++num, num3);
							if (text.Length <= 2 || text[0] != 'b' || text[1] != '4')
							{
								try
								{
									Packet packet = new Packet(text);
									OnPacket(packet);
								}
								catch (Exception ex)
								{
									HTTPManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket", ex);
									((IManager)Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
								}
							}
							else
							{
								byte[] data = Convert.FromBase64String(text.Substring(2));
								if (PacketWithAttachment != null)
								{
									PacketWithAttachment.AddAttachmentFromServer(data, copyFull: true);
									if (PacketWithAttachment.HasAllAttachment)
									{
										try
										{
											OnPacket(PacketWithAttachment);
										}
										catch (Exception ex2)
										{
											HTTPManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket with attachment", ex2);
											((IManager)Manager).EmitError(SocketIOErrors.Internal, ex2.Message + " " + ex2.StackTrace);
										}
										finally
										{
											PacketWithAttachment = null;
										}
									}
								}
							}
							num2 = num + num3;
							num = dataAsText.IndexOf(':', num2);
						}
					}
				}
			}
			catch (Exception ex3)
			{
				((IManager)Manager).EmitError(SocketIOErrors.Internal, ex3.Message + " " + ex3.StackTrace);
				HTTPManager.Logger.Exception("PollingTransport", "ParseResponse", ex3);
			}
		}
	}
}
