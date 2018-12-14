using System;
using System.Collections.Generic;
using System.IO;
using VRC.Core.BestHTTP.Extensions;

namespace VRC.Core.BestHTTP.WebSocket.Frames
{
	internal sealed class WebSocketFrameReader
	{
		public bool IsFinal
		{
			get;
			private set;
		}

		public WebSocketFrameTypes Type
		{
			get;
			private set;
		}

		public bool HasMask
		{
			get;
			private set;
		}

		public ulong Length
		{
			get;
			private set;
		}

		public byte[] Mask
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}

		internal void Read(Stream stream)
		{
			byte b = ReadByte(stream);
			IsFinal = ((b & 0x80) != 0);
			Type = (WebSocketFrameTypes)(b & 0xF);
			b = ReadByte(stream);
			HasMask = ((b & 0x80) != 0);
			Length = (ulong)(b & 0x7F);
			if (Length == 126)
			{
				byte[] array = new byte[2];
				stream.ReadBuffer(array);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(array, 0, array.Length);
				}
				Length = BitConverter.ToUInt16(array, 0);
			}
			else if (Length == 127)
			{
				byte[] array2 = new byte[8];
				stream.ReadBuffer(array2);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(array2, 0, array2.Length);
				}
				Length = BitConverter.ToUInt64(array2, 0);
			}
			if (HasMask)
			{
				Mask = new byte[4];
				if (stream.Read(Mask, 0, 4) < Mask.Length)
				{
					throw ExceptionHelper.ServerClosedTCPStream();
				}
			}
			Data = new byte[Length];
			if (Length != 0L)
			{
				int num = 0;
				do
				{
					int num2 = stream.Read(Data, num, Data.Length - num);
					if (num2 <= 0)
					{
						throw ExceptionHelper.ServerClosedTCPStream();
					}
					num += num2;
				}
				while (num < Data.Length);
				if (HasMask)
				{
					for (int i = 0; i < Data.Length; i++)
					{
						Data[i] = (byte)(Data[i] ^ Mask[i % 4]);
					}
				}
			}
		}

		internal void Assemble(List<WebSocketFrameReader> fragments)
		{
			fragments.Add(this);
			ulong num = 0uL;
			for (int i = 0; i < fragments.Count; i++)
			{
				num += fragments[i].Length;
			}
			byte[] array = new byte[num];
			ulong num2 = 0uL;
			for (int j = 0; j < fragments.Count; j++)
			{
				Array.Copy(fragments[j].Data, 0, array, (int)num2, (int)fragments[j].Length);
				num2 += fragments[j].Length;
			}
			Type = fragments[0].Type;
			Length = num;
			Data = array;
		}

		private byte ReadByte(Stream stream)
		{
			int num = stream.ReadByte();
			if (num < 0)
			{
				throw ExceptionHelper.ServerClosedTCPStream();
			}
			return (byte)num;
		}
	}
}
