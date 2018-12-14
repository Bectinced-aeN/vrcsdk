using System;
using VRC.Core.BestHTTP.ServerSentEvents;
using VRC.Core.BestHTTP.SignalR.Messages;

namespace VRC.Core.BestHTTP.SignalR.Transports
{
	internal sealed class ServerSentEventsTransport : PostSendTransportBase
	{
		private EventSource EventSource;

		public override bool SupportsKeepAlive => true;

		public override TransportTypes Type => TransportTypes.ServerSentEvents;

		public ServerSentEventsTransport(Connection con)
			: base("serverSentEvents", con)
		{
		}

		public override void Connect()
		{
			if (EventSource != null)
			{
				HTTPManager.Logger.Warning("ServerSentEventsTransport", "Start - EventSource already created!");
			}
			else
			{
				if (base.State != TransportStates.Reconnecting)
				{
					base.State = TransportStates.Connecting;
				}
				RequestTypes type = (base.State != TransportStates.Reconnecting) ? RequestTypes.Connect : RequestTypes.Reconnect;
				Uri uri = base.Connection.BuildUri(type, this);
				EventSource = new EventSource(uri);
				EventSource.OnOpen += OnEventSourceOpen;
				EventSource.OnMessage += OnEventSourceMessage;
				EventSource.OnError += OnEventSourceError;
				EventSource.OnClosed += OnEventSourceClosed;
				EventSource.OnRetry += ((EventSource es) => false);
				EventSource.Open();
			}
		}

		public override void Stop()
		{
			EventSource.OnOpen -= OnEventSourceOpen;
			EventSource.OnMessage -= OnEventSourceMessage;
			EventSource.OnError -= OnEventSourceError;
			EventSource.OnClosed -= OnEventSourceClosed;
			EventSource.Close();
			EventSource = null;
		}

		protected override void Started()
		{
		}

		public override void Abort()
		{
			base.Abort();
			EventSource.Close();
		}

		protected override void Aborted()
		{
			if (base.State == TransportStates.Closing)
			{
				base.State = TransportStates.Closed;
			}
		}

		private void OnEventSourceOpen(EventSource eventSource)
		{
			HTTPManager.Logger.Information("Transport - " + base.Name, "OnEventSourceOpen");
		}

		private void OnEventSourceMessage(EventSource eventSource, Message message)
		{
			if (message.Data.Equals("initialized"))
			{
				OnConnected();
			}
			else
			{
				IServerMessage serverMessage = TransportBase.Parse(base.Connection.JsonEncoder, message.Data);
				if (serverMessage != null)
				{
					base.Connection.OnMessage(serverMessage);
				}
			}
		}

		private void OnEventSourceError(EventSource eventSource, string error)
		{
			HTTPManager.Logger.Information("Transport - " + base.Name, "OnEventSourceError");
			if (base.State == TransportStates.Reconnecting)
			{
				Connect();
			}
			else if (base.State != TransportStates.Closed)
			{
				if (base.State == TransportStates.Closing)
				{
					base.State = TransportStates.Closed;
				}
				else
				{
					base.Connection.Error(error);
				}
			}
		}

		private void OnEventSourceClosed(EventSource eventSource)
		{
			HTTPManager.Logger.Information("Transport - " + base.Name, "OnEventSourceClosed");
			OnEventSourceError(eventSource, "EventSource Closed!");
		}
	}
}
