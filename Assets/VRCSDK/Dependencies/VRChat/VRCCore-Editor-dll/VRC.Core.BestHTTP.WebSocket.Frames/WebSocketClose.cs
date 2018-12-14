using System;
using System.IO;
using System.Text;

namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	internal sealed class WebSocketClose : WebSocketBinaryFrame
	{
		public override WebSocketFrameTypes Type => WebSocketFrameTypes.ConnectionClose;

		public WebSocketClose()
			: base(null)
		{
		}

		public WebSocketClose(ushort code, string message)
			: base(GetData(code, message))
		{
		}

		private static byte[] GetData(ushort code, string message)
		{
			int byteCount = Encoding.UTF8.GetByteCount(message);
			using (MemoryStream memoryStream = new MemoryStream(2 + byteCount))
			{
				byte[] bytes = BitConverter.GetBytes(code);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes, 0, bytes.Length);
				}
				memoryStream.Write(bytes, 0, bytes.Length);
				bytes = Encoding.UTF8.GetBytes(message);
				memoryStream.Write(bytes, 0, bytes.Length);
				return memoryStream.ToArray();
				IL_005e:
				byte[] result;
				return result;
			}
		}
	}
}
