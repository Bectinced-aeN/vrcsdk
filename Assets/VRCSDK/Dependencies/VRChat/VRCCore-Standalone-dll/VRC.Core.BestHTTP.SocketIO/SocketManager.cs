using System;
using System.Collections.Generic;
using System.Threading;
using VRC.Core.BestHTTP.Extensions;
using VRC.Core.BestHTTP.SocketIO.Events;
using VRC.Core.BestHTTP.SocketIO.JsonEncoders;
using VRC.Core.BestHTTP.SocketIO.Transports;

namespace VRC.Core.BestHTTP.SocketIO
{
	internal sealed class SocketManager : IHeartbeat, IManager
	{
		public enum States
		{
			Initial,
			Closed,
			Opening,
			Open,
			Reconnecting
		}

		public const int MinProtocolVersion = 4;

		public static IJsonEncoder DefaultEncoder = new DefaultJSonEncoder();

		private States state;

		private int nextAckId;

		private Dictionary<string, Socket> Namespaces = new Dictionary<string, Socket>();

		private List<Socket> Sockets = new List<Socket>();

		private List<Packet> OfflinePackets;

		private DateTime LastHeartbeat = DateTime.MinValue;

		private DateTime LastPongReceived = DateTime.MinValue;

		private DateTime ReconnectAt;

		private DateTime ConnectionStarted;

		private bool closing;

		public States State
		{
			get
			{
				return state;
			}
			private set
			{
				PreviousState = state;
				state = value;
			}
		}

		public SocketOptions Options
		{
			get;
			private set;
		}

		public Uri Uri
		{
			get;
			private set;
		}

		public HandshakeData Handshake
		{
			get;
			private set;
		}

		public ITransport Transport
		{
			get;
			private set;
		}

		public ulong RequestCounter
		{
			get;
			internal set;
		}

		public Socket Socket => GetSocket();

		public Socket this[string nsp]
		{
			get
			{
				return GetSocket(nsp);
			}
		}

		public int ReconnectAttempts
		{
			get;
			private set;
		}

		public IJsonEncoder Encoder
		{
			get;
			set;
		}

		internal uint Timestamp => (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

		internal int NextAckId => Interlocked.Increment(ref nextAckId);

		internal States PreviousState
		{
			get;
			private set;
		}

		public SocketManager(Uri uri)
			: this(uri, new SocketOptions())
		{
		}

		public SocketManager(Uri uri, SocketOptions options)
		{
			Uri = uri;
			Options = options;
			State = States.Initial;
			PreviousState = States.Initial;
			Encoder = DefaultEncoder;
		}

		void IManager.Remove(Socket socket)
		{
			Namespaces.Remove(socket.Namespace);
			Sockets.Remove(socket);
			if (Sockets.Count == 0)
			{
				Close();
			}
		}

		void IManager.Close(bool removeSockets)
		{
			if (State != States.Closed && !closing)
			{
				closing = true;
				HTTPManager.Logger.Information("SocketManager", "Closing");
				HTTPManager.Heartbeats.Unsubscribe(this);
				if (removeSockets)
				{
					while (Sockets.Count > 0)
					{
						((ISocket)Sockets[Sockets.Count - 1]).Disconnect(removeSockets);
					}
				}
				else
				{
					for (int i = 0; i < Sockets.Count; i++)
					{
						((ISocket)Sockets[i]).Disconnect(removeSockets);
					}
				}
				State = States.Closed;
				LastHeartbeat = DateTime.MinValue;
				if (OfflinePackets != null)
				{
					OfflinePackets.Clear();
				}
				if (removeSockets)
				{
					Namespaces.Clear();
				}
				if (Handshake != null)
				{
					Handshake.Abort();
				}
				Handshake = null;
				if (Transport != null)
				{
					Transport.Close();
				}
				Transport = null;
				closing = false;
			}
		}

		void IManager.TryToReconnect()
		{
			if (State != States.Reconnecting && State != States.Closed)
			{
				if (!Options.Reconnection)
				{
					Close();
				}
				else if (++ReconnectAttempts >= Options.ReconnectionAttempts)
				{
					((IManager)this).EmitEvent("reconnect_failed", new object[0]);
					Close();
				}
				else
				{
					Random random = new Random();
					int num = (int)Options.ReconnectionDelay.TotalMilliseconds * ReconnectAttempts;
					ReconnectAt = DateTime.UtcNow + TimeSpan.FromMilliseconds((double)Math.Min(random.Next((int)((float)num - (float)num * Options.RandomizationFactor), (int)((float)num + (float)num * Options.RandomizationFactor)), (int)Options.ReconnectionDelayMax.TotalMilliseconds));
					((IManager)this).Close(removeSockets: false);
					State = States.Reconnecting;
					for (int i = 0; i < Sockets.Count; i++)
					{
						((ISocket)Sockets[i]).Open();
					}
					HTTPManager.Heartbeats.Subscribe(this);
					HTTPManager.Logger.Information("SocketManager", "Reconnecting");
				}
			}
		}

		bool IManager.OnTransportConnected(ITransport trans)
		{
			if (State != States.Opening)
			{
				return false;
			}
			if (PreviousState == States.Reconnecting)
			{
				((IManager)this).EmitEvent("reconnect", new object[0]);
			}
			State = States.Open;
			LastPongReceived = DateTime.UtcNow;
			ReconnectAttempts = 0;
			SendOfflinePackets();
			HTTPManager.Logger.Information("SocketManager", "Open");
			return true;
		}

		void IManager.OnTransportError(ITransport trans, string err)
		{
			((IManager)this).EmitError(SocketIOErrors.Internal, err);
			if (trans.State == TransportStates.Connecting || trans.State == TransportStates.Opening)
			{
				if (trans is WebSocketTransport)
				{
					trans.Close();
					Transport = new PollingTransport(this);
					Transport.Open();
				}
				else
				{
					((IManager)this).TryToReconnect();
				}
			}
			else
			{
				trans.Close();
				((IManager)this).TryToReconnect();
			}
		}

		void IManager.SendPacket(Packet packet)
		{
			ITransport transport = SelectTransport();
			if (transport != null)
			{
				try
				{
					transport.Send(packet);
				}
				catch (Exception ex)
				{
					((IManager)this).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
				}
			}
			else
			{
				if (OfflinePackets == null)
				{
					OfflinePackets = new List<Packet>();
				}
				OfflinePackets.Add(packet.Clone());
			}
		}

		void IManager.OnPacket(Packet packet)
		{
			if (State != States.Closed)
			{
				switch (packet.TransportEvent)
				{
				case TransportEventTypes.Ping:
					((IManager)this).SendPacket(new Packet(TransportEventTypes.Pong, SocketIOEventTypes.Unknown, "/", string.Empty));
					break;
				case TransportEventTypes.Pong:
					LastPongReceived = DateTime.UtcNow;
					break;
				}
				Socket value = null;
				if (Namespaces.TryGetValue(packet.Namespace, out value))
				{
					((ISocket)value).OnPacket(packet);
				}
				else
				{
					HTTPManager.Logger.Warning("SocketManager", "Namespace \"" + packet.Namespace + "\" not found!");
				}
			}
		}

		void IManager.EmitEvent(string eventName, params object[] args)
		{
			Socket value = null;
			if (Namespaces.TryGetValue("/", out value))
			{
				((ISocket)value).EmitEvent(eventName, args);
			}
		}

		void IManager.EmitEvent(SocketIOEventTypes type, params object[] args)
		{
			((IManager)this).EmitEvent(EventNames.GetNameFor(type), args);
		}

		void IManager.EmitError(SocketIOErrors errCode, string msg)
		{
			((IManager)this).EmitEvent(SocketIOEventTypes.Error, new object[1]
			{
				new Error(errCode, msg)
			});
		}

		void IManager.EmitAll(string eventName, params object[] args)
		{
			for (int i = 0; i < Sockets.Count; i++)
			{
				((ISocket)Sockets[i]).EmitEvent(eventName, args);
			}
		}

		void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
		{
			switch (State)
			{
			case States.Opening:
				if (DateTime.UtcNow - ConnectionStarted >= Options.Timeout)
				{
					((IManager)this).EmitError(SocketIOErrors.Internal, "Connection timed out!");
					((IManager)this).EmitEvent("connect_error", new object[0]);
					((IManager)this).EmitEvent("connect_timeout", new object[0]);
					((IManager)this).TryToReconnect();
				}
				break;
			case States.Reconnecting:
				if (ReconnectAt != DateTime.MinValue && DateTime.UtcNow >= ReconnectAt)
				{
					((IManager)this).EmitEvent("reconnect_attempt", new object[0]);
					((IManager)this).EmitEvent("reconnecting", new object[0]);
					Open();
				}
				break;
			case States.Open:
			{
				ITransport transport = null;
				if (Transport != null && Transport.State == TransportStates.Open)
				{
					transport = Transport;
				}
				if (transport != null && transport.State == TransportStates.Open)
				{
					transport.Poll();
					SendOfflinePackets();
					if (LastHeartbeat == DateTime.MinValue)
					{
						LastHeartbeat = DateTime.UtcNow;
					}
					else
					{
						if (DateTime.UtcNow - LastHeartbeat > Handshake.PingInterval)
						{
							((IManager)this).SendPacket(new Packet(TransportEventTypes.Ping, SocketIOEventTypes.Unknown, "/", string.Empty));
							LastHeartbeat = DateTime.UtcNow;
						}
						if (DateTime.UtcNow - LastPongReceived > Handshake.PingTimeout)
						{
							((IManager)this).TryToReconnect();
						}
					}
				}
				break;
			}
			}
		}

		public Socket GetSocket()
		{
			return GetSocket("/");
		}

		public Socket GetSocket(string nsp)
		{
			if (string.IsNullOrEmpty(nsp))
			{
				throw new ArgumentNullException("Namespace parameter is null or empty!");
			}
			Socket value = null;
			if (!Namespaces.TryGetValue(nsp, out value))
			{
				value = new Socket(nsp, this);
				Namespaces.Add(nsp, value);
				Sockets.Add(value);
				((ISocket)value).Open();
			}
			return value;
		}

		public void Open()
		{
			if (State == States.Initial || State == States.Closed || State == States.Reconnecting)
			{
				HTTPManager.Logger.Information("SocketManager", "Opening");
				ReconnectAt = DateTime.MinValue;
				Handshake = new HandshakeData(this);
				Handshake.OnReceived = delegate
				{
					CreateTransports();
				};
				Handshake.OnError = delegate(HandshakeData hsd, string err)
				{
					((IManager)this).EmitError(SocketIOErrors.Internal, err);
					((IManager)this).TryToReconnect();
				};
				Handshake.Start();
				((IManager)this).EmitEvent("connecting", new object[0]);
				State = States.Opening;
				ConnectionStarted = DateTime.UtcNow;
				HTTPManager.Heartbeats.Subscribe(this);
				GetSocket("/");
			}
		}

		public void Close()
		{
			((IManager)this).Close(removeSockets: true);
		}

		private void CreateTransports()
		{
			if (Handshake.Upgrades.Contains("websocket"))
			{
				Transport = new WebSocketTransport(this);
			}
			else
			{
				Transport = new PollingTransport(this);
			}
			Transport.Open();
		}

		private ITransport SelectTransport()
		{
			if (State == States.Open)
			{
				object result;
				if (Transport.IsRequestInProgress)
				{
					ITransport transport = null;
					result = transport;
				}
				else
				{
					result = Transport;
				}
				return (ITransport)result;
			}
			return null;
		}

		private void SendOfflinePackets()
		{
			ITransport transport = SelectTransport();
			if (OfflinePackets != null && OfflinePackets.Count > 0 && transport != null)
			{
				transport.Send(OfflinePackets);
				OfflinePackets.Clear();
			}
		}

		public void EmitAll(string eventName, params object[] args)
		{
			for (int i = 0; i < Sockets.Count; i++)
			{
				Sockets[i].Emit(eventName, args);
			}
		}
	}
}
