using System;
using System.Collections.Generic;
using VRC.Core.BestHTTP.JSON;

namespace VRC.Core.BestHTTP.SocketIO
{
	internal sealed class HandshakeData
	{
		public Action<HandshakeData> OnReceived;

		public Action<HandshakeData, string> OnError;

		private HTTPRequest HandshakeRequest;

		public string Sid
		{
			get;
			private set;
		}

		public List<string> Upgrades
		{
			get;
			private set;
		}

		public TimeSpan PingInterval
		{
			get;
			private set;
		}

		public TimeSpan PingTimeout
		{
			get;
			private set;
		}

		public SocketManager Manager
		{
			get;
			private set;
		}

		public HandshakeData(SocketManager manager)
		{
			Manager = manager;
		}

		internal void Start()
		{
			if (HandshakeRequest == null)
			{
				HandshakeRequest = new HTTPRequest(new Uri($"{Manager.Uri.ToString()}?EIO={4}&transport=polling&t={Manager.Timestamp}-{Manager.RequestCounter++}{Manager.Options.BuildQueryParams()}&b64=true"), OnHandshakeCallback);
				HandshakeRequest.DisableCache = true;
				HandshakeRequest.Send();
				HTTPManager.Logger.Information("HandshakeData", "Handshake request sent");
			}
		}

		internal void Abort()
		{
			if (HandshakeRequest != null)
			{
				HandshakeRequest.Abort();
			}
			HandshakeRequest = null;
			OnReceived = null;
			OnError = null;
		}

		private void OnHandshakeCallback(HTTPRequest req, HTTPResponse resp)
		{
			HandshakeRequest = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (resp.IsSuccess)
				{
					HTTPManager.Logger.Information("HandshakeData", "Handshake data arrived: " + resp.DataAsText);
					int num = resp.DataAsText.IndexOf("{");
					if (num < 0)
					{
						RaiseOnError("Invalid handshake text: " + resp.DataAsText);
					}
					else
					{
						HandshakeData handshakeData = Parse(resp.DataAsText.Substring(num));
						if (handshakeData == null)
						{
							RaiseOnError("Parsing Handshake data failed: " + resp.DataAsText);
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
					RaiseOnError($"Handshake request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText} Uri: {req.CurrentUri}");
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
			HTTPManager.Logger.Error("HandshakeData", "Handshake request failed with error: " + err);
			if (OnError != null)
			{
				OnError(this, err);
				OnError = null;
			}
		}

		private HandshakeData Parse(string str)
		{
			bool success = false;
			Dictionary<string, object> from = Json.Decode(str, ref success) as Dictionary<string, object>;
			if (success)
			{
				try
				{
					Sid = GetString(from, "sid");
					Upgrades = GetStringList(from, "upgrades");
					PingInterval = TimeSpan.FromMilliseconds((double)GetInt(from, "pingInterval"));
					PingTimeout = TimeSpan.FromMilliseconds((double)GetInt(from, "pingTimeout"));
					return this;
				}
				catch
				{
					return null;
					IL_0075:
					return this;
				}
			}
			return null;
		}

		private static object Get(Dictionary<string, object> from, string key)
		{
			if (!from.TryGetValue(key, out object value))
			{
				throw new Exception($"Can't get {key} from Handshake data!");
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
	}
}
