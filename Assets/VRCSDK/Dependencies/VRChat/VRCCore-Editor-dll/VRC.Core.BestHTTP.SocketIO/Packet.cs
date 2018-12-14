using System;
using System.Collections.Generic;
using System.Text;
using VRC.Core.BestHTTP.JSON;
using VRC.Core.BestHTTP.SocketIO.JsonEncoders;

namespace VRC.Core.BestHTTP.SocketIO
{
	internal sealed class Packet
	{
		private enum PayloadTypes : byte
		{
			Textual,
			Binary
		}

		public const string Placeholder = "_placeholder";

		private List<byte[]> attachments;

		public TransportEventTypes TransportEvent
		{
			get;
			private set;
		}

		public SocketIOEventTypes SocketIOEvent
		{
			get;
			private set;
		}

		public int AttachmentCount
		{
			get;
			private set;
		}

		public int Id
		{
			get;
			private set;
		}

		public string Namespace
		{
			get;
			private set;
		}

		public string Payload
		{
			get;
			private set;
		}

		public string EventName
		{
			get;
			private set;
		}

		public List<byte[]> Attachments
		{
			get
			{
				return attachments;
			}
			set
			{
				attachments = value;
				AttachmentCount = ((attachments != null) ? attachments.Count : 0);
			}
		}

		public bool HasAllAttachment => Attachments != null && Attachments.Count == AttachmentCount;

		public bool IsDecoded
		{
			get;
			private set;
		}

		public object[] DecodedArgs
		{
			get;
			private set;
		}

		internal Packet()
		{
			TransportEvent = TransportEventTypes.Unknown;
			SocketIOEvent = SocketIOEventTypes.Unknown;
			Payload = string.Empty;
		}

		internal Packet(string from)
		{
			Parse(from);
		}

		internal Packet(TransportEventTypes transportEvent, SocketIOEventTypes packetType, string nsp, string payload, int attachment = 0, int id = 0)
		{
			TransportEvent = transportEvent;
			SocketIOEvent = packetType;
			Namespace = nsp;
			Payload = payload;
			AttachmentCount = attachment;
			Id = id;
		}

		public object[] Decode(IJsonEncoder encoder)
		{
			if (IsDecoded || encoder == null)
			{
				return DecodedArgs;
			}
			IsDecoded = true;
			if (string.IsNullOrEmpty(Payload))
			{
				return DecodedArgs;
			}
			List<object> list = encoder.Decode(Payload);
			if (list != null && list.Count > 0)
			{
				if (SocketIOEvent == SocketIOEventTypes.Ack || SocketIOEvent == SocketIOEventTypes.BinaryAck)
				{
					DecodedArgs = list.ToArray();
				}
				else
				{
					list.RemoveAt(0);
					DecodedArgs = list.ToArray();
				}
			}
			return DecodedArgs;
		}

		public string DecodeEventName()
		{
			if (!string.IsNullOrEmpty(EventName))
			{
				return EventName;
			}
			if (string.IsNullOrEmpty(Payload))
			{
				return string.Empty;
			}
			if (Payload[0] != '[')
			{
				return string.Empty;
			}
			int i;
			for (i = 1; Payload.Length > i && Payload[i] != '"' && Payload[i] != '\''; i++)
			{
			}
			if (Payload.Length <= i)
			{
				return string.Empty;
			}
			int num = ++i;
			for (; Payload.Length > i && Payload[i] != '"' && Payload[i] != '\''; i++)
			{
			}
			if (Payload.Length <= i)
			{
				return string.Empty;
			}
			return EventName = Payload.Substring(num, i - num);
		}

		public string RemoveEventName(bool removeArrayMarks)
		{
			if (string.IsNullOrEmpty(Payload))
			{
				return string.Empty;
			}
			if (Payload[0] != '[')
			{
				return string.Empty;
			}
			int i;
			for (i = 1; Payload.Length > i && Payload[i] != '"' && Payload[i] != '\''; i++)
			{
			}
			if (Payload.Length <= i)
			{
				return string.Empty;
			}
			int num = i;
			for (; Payload.Length > i && Payload[i] != ',' && Payload[i] != ']'; i++)
			{
			}
			if (Payload.Length <= ++i)
			{
				return string.Empty;
			}
			string text = Payload.Remove(num, i - num);
			if (removeArrayMarks)
			{
				text = text.Substring(1, text.Length - 2);
			}
			return text;
		}

		public bool ReconstructAttachmentAsIndex()
		{
			return PlaceholderReplacer(delegate(string json, Dictionary<string, object> obj)
			{
				int num = Convert.ToInt32(obj["num"]);
				Payload = Payload.Replace(json, num.ToString());
				IsDecoded = false;
			});
		}

		public bool ReconstructAttachmentAsBase64()
		{
			if (!HasAllAttachment)
			{
				return false;
			}
			return PlaceholderReplacer(delegate(string json, Dictionary<string, object> obj)
			{
				int index = Convert.ToInt32(obj["num"]);
				Payload = Payload.Replace(json, $"\"{Convert.ToBase64String(Attachments[index])}\"");
				IsDecoded = false;
			});
		}

		internal void Parse(string from)
		{
			int i = 0;
			TransportEvent = (TransportEventTypes)char.GetNumericValue(from, i++);
			if (from.Length > i && char.GetNumericValue(from, i) >= 0.0)
			{
				SocketIOEvent = (SocketIOEventTypes)char.GetNumericValue(from, i++);
			}
			else
			{
				SocketIOEvent = SocketIOEventTypes.Unknown;
			}
			if (SocketIOEvent == SocketIOEventTypes.BinaryEvent || SocketIOEvent == SocketIOEventTypes.BinaryAck)
			{
				int num3 = from.IndexOf('-', i);
				if (num3 == -1)
				{
					num3 = from.Length;
				}
				int result = 0;
				int.TryParse(from.Substring(i, num3 - i), out result);
				AttachmentCount = result;
				i = num3 + 1;
			}
			if (from.Length > i && from[i] == '/')
			{
				int num4 = from.IndexOf(',', i);
				if (num4 == -1)
				{
					num4 = from.Length;
				}
				Namespace = from.Substring(i, num4 - i);
				i = num4 + 1;
			}
			else
			{
				Namespace = "/";
			}
			if (from.Length > i && char.GetNumericValue(from[i]) >= 0.0)
			{
				int num6 = i++;
				for (; from.Length > i && char.GetNumericValue(from[i]) >= 0.0; i++)
				{
				}
				int result2 = 0;
				int.TryParse(from.Substring(num6, i - num6), out result2);
				Id = result2;
			}
			if (from.Length > i)
			{
				Payload = from.Substring(i);
			}
			else
			{
				Payload = string.Empty;
			}
		}

		internal string Encode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (TransportEvent == TransportEventTypes.Unknown && AttachmentCount > 0)
			{
				TransportEvent = TransportEventTypes.Message;
			}
			if (TransportEvent != TransportEventTypes.Unknown)
			{
				stringBuilder.Append(((int)TransportEvent).ToString());
			}
			if (SocketIOEvent == SocketIOEventTypes.Unknown && AttachmentCount > 0)
			{
				SocketIOEvent = SocketIOEventTypes.BinaryEvent;
			}
			if (SocketIOEvent != SocketIOEventTypes.Unknown)
			{
				stringBuilder.Append(((int)SocketIOEvent).ToString());
			}
			if (SocketIOEvent == SocketIOEventTypes.BinaryEvent || SocketIOEvent == SocketIOEventTypes.BinaryAck)
			{
				stringBuilder.Append(AttachmentCount.ToString());
				stringBuilder.Append("-");
			}
			bool flag = false;
			if (Namespace != "/")
			{
				stringBuilder.Append(Namespace);
				flag = true;
			}
			if (Id != 0)
			{
				if (flag)
				{
					stringBuilder.Append(",");
					flag = false;
				}
				stringBuilder.Append(Id.ToString());
			}
			if (!string.IsNullOrEmpty(Payload))
			{
				if (flag)
				{
					stringBuilder.Append(",");
					flag = false;
				}
				stringBuilder.Append(Payload);
			}
			return stringBuilder.ToString();
		}

		internal byte[] EncodeBinary()
		{
			if (AttachmentCount != 0 || (Attachments != null && Attachments.Count != 0))
			{
				if (Attachments == null)
				{
					throw new ArgumentException("packet.Attachments are null!");
				}
				if (AttachmentCount != Attachments.Count)
				{
					throw new ArgumentException("packet.AttachmentCount != packet.Attachments.Count. Use the packet.AddAttachment function to add data to a packet!");
				}
			}
			string s = Encode();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			byte[] array = EncodeData(bytes, PayloadTypes.Textual, null);
			if (AttachmentCount != 0)
			{
				int num = array.Length;
				List<byte[]> list = new List<byte[]>(AttachmentCount);
				int num2 = 0;
				for (int i = 0; i < AttachmentCount; i++)
				{
					byte[] array2 = EncodeData(Attachments[i], PayloadTypes.Binary, new byte[1]
					{
						4
					});
					list.Add(array2);
					num2 += array2.Length;
				}
				Array.Resize(ref array, array.Length + num2);
				for (int j = 0; j < AttachmentCount; j++)
				{
					byte[] array3 = list[j];
					Array.Copy(array3, 0, array, num, array3.Length);
					num += array3.Length;
				}
			}
			return array;
		}

		internal void AddAttachmentFromServer(byte[] data, bool copyFull)
		{
			if (data != null && data.Length != 0)
			{
				if (attachments == null)
				{
					attachments = new List<byte[]>(AttachmentCount);
				}
				if (copyFull)
				{
					Attachments.Add(data);
				}
				else
				{
					byte[] array = new byte[data.Length - 1];
					Array.Copy(data, 1, array, 0, data.Length - 1);
					Attachments.Add(array);
				}
			}
		}

		private byte[] EncodeData(byte[] data, PayloadTypes type, byte[] afterHeaderData)
		{
			int num = (afterHeaderData != null) ? afterHeaderData.Length : 0;
			string text = (data.Length + num).ToString();
			byte[] array = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				array[i] = (byte)char.GetNumericValue(text[i]);
			}
			byte[] array2 = new byte[data.Length + array.Length + 2 + num];
			array2[0] = (byte)type;
			for (int j = 0; j < array.Length; j++)
			{
				array2[1 + j] = array[j];
			}
			int num2 = 1 + array.Length;
			array2[num2++] = byte.MaxValue;
			if (afterHeaderData != null && afterHeaderData.Length > 0)
			{
				Array.Copy(afterHeaderData, 0, array2, num2, afterHeaderData.Length);
				num2 += afterHeaderData.Length;
			}
			Array.Copy(data, 0, array2, num2, data.Length);
			return array2;
		}

		private bool PlaceholderReplacer(Action<string, Dictionary<string, object>> onFound)
		{
			if (string.IsNullOrEmpty(Payload))
			{
				return false;
			}
			for (int num = Payload.IndexOf("_placeholder"); num >= 0; num = Payload.IndexOf("_placeholder"))
			{
				int num2 = num;
				while (Payload[num2] != '{')
				{
					num2--;
				}
				int i;
				for (i = num; Payload.Length > i && Payload[i] != '}'; i++)
				{
				}
				if (Payload.Length <= i)
				{
					return false;
				}
				string text = Payload.Substring(num2, i - num2 + 1);
				bool success = false;
				Dictionary<string, object> dictionary = Json.Decode(text, ref success) as Dictionary<string, object>;
				if (!success)
				{
					return false;
				}
				if (!dictionary.TryGetValue("_placeholder", out object value) || !(bool)value)
				{
					return false;
				}
				if (!dictionary.TryGetValue("num", out value))
				{
					return false;
				}
				onFound(text, dictionary);
			}
			return true;
		}

		public override string ToString()
		{
			return Payload;
		}

		internal Packet Clone()
		{
			Packet packet = new Packet(TransportEvent, SocketIOEvent, Namespace, Payload, 0, Id);
			packet.EventName = EventName;
			packet.AttachmentCount = AttachmentCount;
			packet.attachments = attachments;
			return packet;
		}
	}
}
