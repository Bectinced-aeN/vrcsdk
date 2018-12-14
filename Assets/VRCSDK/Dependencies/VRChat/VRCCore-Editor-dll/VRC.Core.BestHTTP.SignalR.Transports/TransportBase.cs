using System;
using System.Collections.Generic;
using VRC.Core.BestHTTP.SignalR.JsonEncoders;
using VRC.Core.BestHTTP.SignalR.Messages;

namespace VRC.Core.BestHTTP.SignalR.Transports
{
	internal abstract class TransportBase
	{
		private const int MaxRetryCount = 5;

		public TransportStates _state;

		public string Name
		{
			get;
			protected set;
		}

		public abstract bool SupportsKeepAlive
		{
			get;
		}

		public abstract TransportTypes Type
		{
			get;
		}

		public IConnection Connection
		{
			get;
			protected set;
		}

		public TransportStates State
		{
			get
			{
				return _state;
			}
			protected set
			{
				TransportStates state = _state;
				_state = value;
				if (this.OnStateChanged != null)
				{
					this.OnStateChanged(this, state, _state);
				}
			}
		}

		public event OnTransportStateChangedDelegate OnStateChanged;

		public TransportBase(string name, Connection connection)
		{
			Name = name;
			Connection = connection;
			State = TransportStates.Initial;
		}

		public abstract void Connect();

		public abstract void Stop();

		protected abstract void SendImpl(string json);

		protected abstract void Started();

		protected abstract void Aborted();

		protected void OnConnected()
		{
			if (State != TransportStates.Reconnecting)
			{
				Start();
			}
			else
			{
				Connection.TransportReconnected();
				Started();
				State = TransportStates.Started;
			}
		}

		protected void Start()
		{
			HTTPManager.Logger.Information("Transport - " + Name, "Sending Start Request");
			State = TransportStates.Starting;
			if ((int)Connection.Protocol > 0)
			{
				HTTPRequest hTTPRequest = new HTTPRequest(Connection.BuildUri(RequestTypes.Start, this), HTTPMethods.Get, isKeepAlive: true, disableCache: true, OnStartRequestFinished);
				hTTPRequest.Tag = 0;
				hTTPRequest.DisableRetry = true;
				hTTPRequest.Timeout = Connection.NegotiationResult.ConnectionTimeout + TimeSpan.FromSeconds(10.0);
				Connection.PrepareRequest(hTTPRequest, RequestTypes.Start);
				hTTPRequest.Send();
			}
			else
			{
				State = TransportStates.Started;
				Started();
				Connection.TransportStarted();
			}
		}

		private void OnStartRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			HTTPRequestStates state = req.State;
			if (state == HTTPRequestStates.Finished)
			{
				if (resp.IsSuccess)
				{
					HTTPManager.Logger.Information("Transport - " + Name, "Start - Returned: " + resp.DataAsText);
					string text = Connection.ParseResponse(resp.DataAsText);
					if (text != "started")
					{
						Connection.Error($"Expected 'started' response, but '{text}' found!");
					}
					else
					{
						State = TransportStates.Started;
						Started();
						Connection.TransportStarted();
					}
					return;
				}
				HTTPManager.Logger.Warning("Transport - " + Name, $"Start - request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} Uri: {req.CurrentUri}");
			}
			HTTPManager.Logger.Information("Transport - " + Name, "Start request state: " + req.State.ToString());
			int num = (int)req.Tag;
			if (num++ < 5)
			{
				req.Tag = num;
				req.Send();
			}
			else
			{
				Connection.Error("Failed to send Start request.");
			}
		}

		public virtual void Abort()
		{
			if (State == TransportStates.Started)
			{
				State = TransportStates.Closing;
				HTTPRequest hTTPRequest = new HTTPRequest(Connection.BuildUri(RequestTypes.Abort, this), HTTPMethods.Get, isKeepAlive: true, disableCache: true, OnAbortRequestFinished);
				hTTPRequest.Tag = 0;
				hTTPRequest.DisableRetry = true;
				Connection.PrepareRequest(hTTPRequest, RequestTypes.Abort);
				hTTPRequest.Send();
			}
		}

		protected void AbortFinished()
		{
			State = TransportStates.Closed;
			Connection.TransportAborted();
			Aborted();
		}

		private void OnAbortRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			HTTPRequestStates state = req.State;
			if (state == HTTPRequestStates.Finished)
			{
				if (resp.IsSuccess)
				{
					HTTPManager.Logger.Information("Transport - " + Name, "Abort - Returned: " + resp.DataAsText);
					if (State == TransportStates.Closing)
					{
						AbortFinished();
					}
					return;
				}
				HTTPManager.Logger.Warning("Transport - " + Name, $"Abort - Handshake request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} Uri: {req.CurrentUri}");
			}
			HTTPManager.Logger.Information("Transport - " + Name, "Abort request state: " + req.State.ToString());
			int num = (int)req.Tag;
			if (num++ < 5)
			{
				req.Tag = num;
				req.Send();
			}
			else
			{
				Connection.Error("Failed to send Abort request!");
			}
		}

		public void Send(string jsonStr)
		{
			try
			{
				HTTPManager.Logger.Information("Transport - " + Name, "Sending: " + jsonStr);
				SendImpl(jsonStr);
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("Transport - " + Name, "Send", ex);
			}
		}

		public void Reconnect()
		{
			HTTPManager.Logger.Information("Transport - " + Name, "Reconnecting");
			Stop();
			State = TransportStates.Reconnecting;
			Connect();
		}

		public static IServerMessage Parse(IJsonEncoder encoder, string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				HTTPManager.Logger.Error("MessageFactory", "Parse - called with empty or null string!");
				return null;
			}
			if (json.Length == 2 && json == "{}")
			{
				return new KeepAliveMessage();
			}
			IDictionary<string, object> dictionary = null;
			try
			{
				dictionary = encoder.DecodeMessage(json);
			}
			catch (Exception ex)
			{
				HTTPManager.Logger.Exception("MessageFactory", "Parse - encoder.DecodeMessage", ex);
				return null;
				IL_006f:;
			}
			if (dictionary == null)
			{
				HTTPManager.Logger.Error("MessageFactory", "Parse - Json Decode failed for json string: \"" + json + "\"");
				return null;
			}
			IServerMessage serverMessage = null;
			serverMessage = (IServerMessage)(dictionary.ContainsKey("C") ? new MultiMessage() : (dictionary.ContainsKey("E") ? ((object)new FailureMessage()) : ((object)new ResultMessage())));
			serverMessage.Parse(dictionary);
			return serverMessage;
		}
	}
}
