using System;
using System.Collections.Generic;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core.BestHTTP.SignalR
{
	internal sealed class NegotiationData
	{
		public Action<NegotiationData> OnReceived;

		public Action<NegotiationData, string> OnError;

		private HTTPRequest NegotiationRequest;

		private IConnection Connection;

		public string Url
		{
			get;
			private set;
		}

		public string WebSocketServerUrl
		{
			get;
			private set;
		}

		public string ConnectionToken
		{
			get;
			private set;
		}

		public string ConnectionId
		{
			get;
			private set;
		}

		public TimeSpan? KeepAliveTimeout
		{
			get;
			private set;
		}

		public TimeSpan DisconnectTimeout
		{
			get;
			private set;
		}

		public TimeSpan ConnectionTimeout
		{
			get;
			private set;
		}

		public bool TryWebSockets
		{
			get;
			private set;
		}

		public string ProtocolVersion
		{
			get;
			private set;
		}

		public TimeSpan TransportConnectTimeout
		{
			get;
			private set;
		}

		public TimeSpan LongPollDelay
		{
			get;
			private set;
		}

		public NegotiationData(Connection connection)
		{
			Connection = connection;
		}

		public void Start()
		{
			NegotiationRequest = new HTTPRequest(Connection.BuildUri(RequestTypes.Negotiate), HTTPMethods.Get, isKeepAlive: true, disableCache: true, OnNegotiationRequestFinished);
			Connection.PrepareRequest(NegotiationRequest, RequestTypes.Negotiate);
			NegotiationRequest.Send();
			HTTPManager.Logger.Information("NegotiationData", "Negotiation request sent");
		}

		public void Abort()
		{
			if (NegotiationRequest != null)
			{
				OnReceived = null;
				OnError = null;
				NegotiationRequest.Abort();
			}
		}

		private void OnNegotiationRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			NegotiationRequest = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (resp.IsSuccess)
				{
					HTTPManager.Logger.Information("NegotiationData", "Negotiation data arrived: " + resp.DataAsText);
					int num = resp.DataAsText.IndexOf("{");
					if (num < 0)
					{
						RaiseOnError("Invalid negotiation text: " + resp.DataAsText);
					}
					else
					{
						NegotiationData negotiationData = Parse(resp.DataAsText.Substring(num));
						if (negotiationData == null)
						{
							RaiseOnError("Parsing Negotiation data failed: " + resp.DataAsText);
						}
						else if (OnReceived != null)
						{
							OnReceived(this);
							OnReceived = null;
						}
					}
				}
				else
				{
					RaiseOnError($"Negotiation request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} Uri: {req.CurrentUri}");
				}
				break;
			case HTTPRequestStates.Error:
				RaiseOnError((req.Exception == null) ? string.Empty : (req.Exception.Message + " " + req.Exception.StackTrace));
				break;
			default:
				RaiseOnError(req.State.ToString());
				break;
			}
		}

		private void RaiseOnError(string err)
		{
			HTTPManager.Logger.Error("NegotiationData", "Negotiation request failed with error: " + err);
			if (OnError != null)
			{
				OnError(this, err);
				OnError = null;
			}
		}

		private NegotiationData Parse(string str)
		{
			bool success = false;
			Dictionary<string, object> dictionary = Json.Decode(str, ref success) as Dictionary<string, object>;
			if (success)
			{
				try
				{
					Url = GetString(dictionary, "Url");
					if (dictionary.ContainsKey("webSocketServerUrl"))
					{
						WebSocketServerUrl = GetString(dictionary, "webSocketServerUrl");
					}
					ConnectionToken = Uri.EscapeDataString(GetString(dictionary, "ConnectionToken"));
					ConnectionId = GetString(dictionary, "ConnectionId");
					if (dictionary.ContainsKey("KeepAliveTimeout"))
					{
						KeepAliveTimeout = TimeSpan.FromSeconds(GetDouble(dictionary, "KeepAliveTimeout"));
					}
					DisconnectTimeout = TimeSpan.FromSeconds(GetDouble(dictionary, "DisconnectTimeout"));
					if (dictionary.ContainsKey("ConnectionTimeout"))
					{
						ConnectionTimeout = TimeSpan.FromSeconds(GetDouble(dictionary, "ConnectionTimeout"));
					}
					else
					{
						ConnectionTimeout = TimeSpan.FromSeconds(120.0);
					}
					TryWebSockets = (bool)Get(dictionary, "TryWebSockets");
					ProtocolVersion = GetString(dictionary, "ProtocolVersion");
					TransportConnectTimeout = TimeSpan.FromSeconds(GetDouble(dictionary, "TransportConnectTimeout"));
					if (!dictionary.ContainsKey("LongPollDelay"))
					{
						return this;
					}
					LongPollDelay = TimeSpan.FromSeconds(GetDouble(dictionary, "LongPollDelay"));
					return this;
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("NegotiationData", "Parse", ex);
					return null;
					IL_0176:
					return this;
				}
			}
			return null;
		}

		private static object Get(Dictionary<string, object> from, string key)
		{
			if (!from.TryGetValue(key, out object value))
			{
				throw new Exception($"Can't get {key} from Negotiation data!");
			}
			return value;
		}

		private static string GetString(Dictionary<string, object> from, string key)
		{
			return Get(from, key) as string;
		}

		private static List<string> GetStringList(Dictionary<string, object> from, string key)
		{
			List<object> list = Get(from, key) as List<object>;
			List<string> list2 = new List<string>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i] as string;
				if (text != null)
				{
					list2.Add(text);
				}
			}
			return list2;
		}

		private static int GetInt(Dictionary<string, object> from, string key)
		{
			return (int)(double)Get(from, key);
		}

		private static double GetDouble(Dictionary<string, object> from, string key)
		{
			return (double)Get(from, key);
		}
	}
}
