using System.Collections.Generic;
using VRC.Core.BestHTTP.Logger;

namespace VRC.Core.BestHTTP.SocketIO.Events
{
	internal sealed class EventTable
	{
		private Dictionary<string, List<EventDescriptor>> Table = new Dictionary<string, List<EventDescriptor>>();

		private Socket Socket
		{
			get;
			set;
		}

		public EventTable(Socket socket)
		{
			Socket = socket;
		}

		public void Register(string eventName, SocketIOCallback callback, bool onlyOnce, bool autoDecodePayload)
		{
			if (!Table.TryGetValue(eventName, out List<EventDescriptor> value))
			{
				Table.Add(eventName, value = new List<EventDescriptor>(1));
			}
			EventDescriptor eventDescriptor = value.Find((EventDescriptor d) => d.OnlyOnce == onlyOnce && d.AutoDecodePayload == autoDecodePayload);
			if (eventDescriptor == null)
			{
				value.Add(new EventDescriptor(onlyOnce, autoDecodePayload, callback));
			}
			else
			{
				eventDescriptor.Callbacks.Add(callback);
			}
		}

		public void Unregister(string eventName)
		{
			Table.Remove(eventName);
		}

		public void Unregister(string eventName, SocketIOCallback callback)
		{
			if (Table.TryGetValue(eventName, out List<EventDescriptor> value))
			{
				for (int i = 0; i < value.Count; i++)
				{
					value[i].Callbacks.Remove(callback);
				}
			}
		}

		public void Call(string eventName, Packet packet, params object[] args)
		{
			if (HTTPManager.Logger.Level <= Loglevels.All)
			{
				HTTPManager.Logger.Verbose("EventTable", "Call - " + eventName);
			}
			if (Table.TryGetValue(eventName, out List<EventDescriptor> value))
			{
				for (int i = 0; i < value.Count; i++)
				{
					value[i].Call(Socket, packet, args);
				}
			}
		}

		public void Call(Packet packet)
		{
			string text = packet.DecodeEventName();
			string text2 = (packet.SocketIOEvent == SocketIOEventTypes.Unknown) ? EventNames.GetNameFor(packet.TransportEvent) : EventNames.GetNameFor(packet.SocketIOEvent);
			object[] args = null;
			if (HasSubsciber(text) || HasSubsciber(text2))
			{
				if (packet.TransportEvent == TransportEventTypes.Message && (packet.SocketIOEvent == SocketIOEventTypes.Event || packet.SocketIOEvent == SocketIOEventTypes.BinaryEvent) && ShouldDecodePayload(text))
				{
					args = packet.Decode(Socket.Manager.Encoder);
				}
				if (!string.IsNullOrEmpty(text))
				{
					Call(text, packet, args);
				}
				if (!packet.IsDecoded && ShouldDecodePayload(text2))
				{
					args = packet.Decode(Socket.Manager.Encoder);
				}
				if (!string.IsNullOrEmpty(text2))
				{
					Call(text2, packet, args);
				}
			}
		}

		public void Clear()
		{
			Table.Clear();
		}

		private bool ShouldDecodePayload(string eventName)
		{
			if (Table.TryGetValue(eventName, out List<EventDescriptor> value))
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i].AutoDecodePayload && value[i].Callbacks.Count > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasSubsciber(string eventName)
		{
			return Table.ContainsKey(eventName);
		}
	}
}
