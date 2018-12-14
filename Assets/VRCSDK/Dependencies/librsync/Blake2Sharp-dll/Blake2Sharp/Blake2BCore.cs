using System;

namespace Blake2Sharp
{
	public sealed class Blake2BCore
	{
		private bool _isInitialized = false;

		private int _bufferFilled;

		private byte[] _buf = new byte[128];

		private ulong[] _m = new ulong[16];

		private ulong[] _h = new ulong[8];

		private ulong _counter0;

		private ulong _counter1;

		private ulong _finalizationFlag0;

		private ulong _finalizationFlag1;

		private const int NumberOfRounds = 12;

		private const int BlockSizeInBytes = 128;

		private const ulong IV0 = 7640891576956012808uL;

		private const ulong IV1 = 13503953896175478587uL;

		private const ulong IV2 = 4354685564936845355uL;

		private const ulong IV3 = 11912009170470909681uL;

		private const ulong IV4 = 5840696475078001361uL;

		private const ulong IV5 = 11170449401992604703uL;

		private const ulong IV6 = 2270897969802886507uL;

		private const ulong IV7 = 6620516959819538809uL;

		private static readonly int[] Sigma = new int[192]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			14,
			10,
			4,
			8,
			9,
			15,
			13,
			6,
			1,
			12,
			0,
			2,
			11,
			7,
			5,
			3,
			11,
			8,
			12,
			0,
			5,
			2,
			15,
			13,
			10,
			14,
			3,
			6,
			7,
			1,
			9,
			4,
			7,
			9,
			3,
			1,
			13,
			12,
			11,
			14,
			2,
			6,
			5,
			10,
			4,
			0,
			15,
			8,
			9,
			0,
			5,
			7,
			2,
			4,
			10,
			15,
			14,
			1,
			11,
			12,
			6,
			8,
			3,
			13,
			2,
			12,
			6,
			10,
			0,
			11,
			8,
			3,
			4,
			13,
			7,
			5,
			15,
			14,
			1,
			9,
			12,
			5,
			1,
			15,
			14,
			13,
			4,
			10,
			0,
			7,
			6,
			3,
			9,
			2,
			8,
			11,
			13,
			11,
			7,
			14,
			12,
			1,
			3,
			9,
			5,
			0,
			15,
			4,
			8,
			6,
			2,
			10,
			6,
			15,
			14,
			9,
			11,
			3,
			0,
			8,
			12,
			2,
			13,
			7,
			1,
			4,
			10,
			5,
			10,
			2,
			8,
			4,
			7,
			6,
			1,
			5,
			15,
			11,
			9,
			14,
			3,
			12,
			13,
			0,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			14,
			10,
			4,
			8,
			9,
			15,
			13,
			6,
			1,
			12,
			0,
			2,
			11,
			7,
			5,
			3
		};

		internal static ulong BytesToUInt64(byte[] buf, int offset)
		{
			return ((ulong)buf[offset + 7] << 56) | ((ulong)buf[offset + 6] << 48) | ((ulong)buf[offset + 5] << 40) | ((ulong)buf[offset + 4] << 32) | ((ulong)buf[offset + 3] << 24) | ((ulong)buf[offset + 2] << 16) | ((ulong)buf[offset + 1] << 8) | buf[offset];
		}

		private static void UInt64ToBytes(ulong value, byte[] buf, int offset)
		{
			buf[offset + 7] = (byte)(value >> 56);
			buf[offset + 6] = (byte)(value >> 48);
			buf[offset + 5] = (byte)(value >> 40);
			buf[offset + 4] = (byte)(value >> 32);
			buf[offset + 3] = (byte)(value >> 24);
			buf[offset + 2] = (byte)(value >> 16);
			buf[offset + 1] = (byte)(value >> 8);
			buf[offset] = (byte)value;
		}

		private void Compress(byte[] block, int start)
		{
			ulong[] h = _h;
			ulong[] m = _m;
			if (BitConverter.IsLittleEndian)
			{
				Buffer.BlockCopy(block, start, m, 0, 128);
			}
			else
			{
				int num2;
				for (int num = 0; num < 16; num = num2)
				{
					m[num] = BytesToUInt64(block, start + (num << 3));
					num2 = num + 1;
				}
			}
			ulong num3 = h[0];
			ulong num4 = h[1];
			ulong num5 = h[2];
			ulong num6 = h[3];
			ulong num7 = h[4];
			ulong num8 = h[5];
			ulong num9 = h[6];
			ulong num10 = h[7];
			ulong num11 = 7640891576956012808uL;
			ulong num12 = 13503953896175478587uL;
			ulong num13 = 4354685564936845355uL;
			ulong num14 = 11912009170470909681uL;
			ulong num15 = 0x510E527FADE682D1 ^ _counter0;
			ulong num16 = (ulong)(-7276294671716946913L ^ (long)_counter1);
			ulong num17 = 0x1F83D9ABFB41BD6B ^ _finalizationFlag0;
			ulong num18 = 0x5BE0CD19137E2179 ^ _finalizationFlag1;
			num3 = num3 + num7 + m[0];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[1];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[2];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[3];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[4];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[5];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[6];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[7];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[8];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[9];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[10];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[11];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[12];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[13];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[14];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[15];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[14];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[10];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[4];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[8];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[9];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[15];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[13];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[6];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[1];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[12];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[0];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[2];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[11];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[7];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[5];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[3];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[11];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[8];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[12];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[0];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[5];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[2];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[15];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[13];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[10];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[14];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[3];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[6];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[7];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[1];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[9];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[4];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[7];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[9];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[3];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[1];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[13];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[12];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[11];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[14];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[2];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[6];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[5];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[10];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[4];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[0];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[15];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[8];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[9];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[0];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[5];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[7];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[2];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[4];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[10];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[15];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[14];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[1];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[11];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[12];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[6];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[8];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[3];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[13];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[2];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[12];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[6];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[10];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[0];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[11];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[8];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[3];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[4];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[13];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[7];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[5];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[15];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[14];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[1];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[9];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[12];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[5];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[1];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[15];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[14];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[13];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[4];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[10];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[0];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[7];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[6];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[3];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[9];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[2];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[8];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[11];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[13];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[11];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[7];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[14];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[12];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[1];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[3];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[9];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[5];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[0];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[15];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[4];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[8];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[6];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[2];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[10];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[6];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[15];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[14];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[9];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[11];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[3];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[0];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[8];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[12];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[2];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[13];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[7];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[1];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[4];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[10];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[5];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[10];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[2];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[8];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[4];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[7];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[6];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[1];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[5];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[15];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[11];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[9];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[14];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[3];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[12];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[13];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[0];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[0];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[1];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[2];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[3];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[4];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[5];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[6];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[7];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[8];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[9];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[10];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[11];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[12];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[13];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[14];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[15];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			num3 = num3 + num7 + m[14];
			num15 ^= num3;
			num15 = ((num15 >> 32) | (num15 << 32));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 24) | (num7 << 40));
			num3 = num3 + num7 + m[10];
			num15 ^= num3;
			num15 = ((num15 >> 16) | (num15 << 48));
			num11 += num15;
			num7 ^= num11;
			num7 = ((num7 >> 63) | (num7 << 1));
			num4 = num4 + num8 + m[4];
			num16 ^= num4;
			num16 = ((num16 >> 32) | (num16 << 32));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 24) | (num8 << 40));
			num4 = num4 + num8 + m[8];
			num16 ^= num4;
			num16 = ((num16 >> 16) | (num16 << 48));
			num12 += num16;
			num8 ^= num12;
			num8 = ((num8 >> 63) | (num8 << 1));
			num5 = num5 + num9 + m[9];
			num17 ^= num5;
			num17 = ((num17 >> 32) | (num17 << 32));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 24) | (num9 << 40));
			num5 = num5 + num9 + m[15];
			num17 ^= num5;
			num17 = ((num17 >> 16) | (num17 << 48));
			num13 += num17;
			num9 ^= num13;
			num9 = ((num9 >> 63) | (num9 << 1));
			num6 = num6 + num10 + m[13];
			num18 ^= num6;
			num18 = ((num18 >> 32) | (num18 << 32));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 24) | (num10 << 40));
			num6 = num6 + num10 + m[6];
			num18 ^= num6;
			num18 = ((num18 >> 16) | (num18 << 48));
			num14 += num18;
			num10 ^= num14;
			num10 = ((num10 >> 63) | (num10 << 1));
			num3 = num3 + num8 + m[1];
			num18 ^= num3;
			num18 = ((num18 >> 32) | (num18 << 32));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 24) | (num8 << 40));
			num3 = num3 + num8 + m[12];
			num18 ^= num3;
			num18 = ((num18 >> 16) | (num18 << 48));
			num13 += num18;
			num8 ^= num13;
			num8 = ((num8 >> 63) | (num8 << 1));
			num4 = num4 + num9 + m[0];
			num15 ^= num4;
			num15 = ((num15 >> 32) | (num15 << 32));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 24) | (num9 << 40));
			num4 = num4 + num9 + m[2];
			num15 ^= num4;
			num15 = ((num15 >> 16) | (num15 << 48));
			num14 += num15;
			num9 ^= num14;
			num9 = ((num9 >> 63) | (num9 << 1));
			num5 = num5 + num10 + m[11];
			num16 ^= num5;
			num16 = ((num16 >> 32) | (num16 << 32));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 24) | (num10 << 40));
			num5 = num5 + num10 + m[7];
			num16 ^= num5;
			num16 = ((num16 >> 16) | (num16 << 48));
			num11 += num16;
			num10 ^= num11;
			num10 = ((num10 >> 63) | (num10 << 1));
			num6 = num6 + num7 + m[5];
			num17 ^= num6;
			num17 = ((num17 >> 32) | (num17 << 32));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 24) | (num7 << 40));
			num6 = num6 + num7 + m[3];
			num17 ^= num6;
			num17 = ((num17 >> 16) | (num17 << 48));
			num12 += num17;
			num7 ^= num12;
			num7 = ((num7 >> 63) | (num7 << 1));
			ref ulong reference = ref h[0];
			reference ^= (num3 ^ num11);
			reference = ref h[1];
			reference ^= (num4 ^ num12);
			reference = ref h[2];
			reference ^= (num5 ^ num13);
			reference = ref h[3];
			reference ^= (num6 ^ num14);
			reference = ref h[4];
			reference ^= (num7 ^ num15);
			reference = ref h[5];
			reference ^= (num8 ^ num16);
			reference = ref h[6];
			reference ^= (num9 ^ num17);
			reference = ref h[7];
			reference ^= (num10 ^ num18);
		}

		public void Initialize(ulong[] config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (config.Length != 8)
			{
				throw new ArgumentException("config length must be 8 words");
			}
			_isInitialized = true;
			_h[0] = 7640891576956012808uL;
			_h[1] = 13503953896175478587uL;
			_h[2] = 4354685564936845355uL;
			_h[3] = 11912009170470909681uL;
			_h[4] = 5840696475078001361uL;
			_h[5] = 11170449401992604703uL;
			_h[6] = 2270897969802886507uL;
			_h[7] = 6620516959819538809uL;
			_counter0 = 0uL;
			_counter1 = 0uL;
			_finalizationFlag0 = 0uL;
			_finalizationFlag1 = 0uL;
			_bufferFilled = 0;
			Array.Clear(_buf, 0, _buf.Length);
			int num2;
			for (int num = 0; num < 8; num = num2 + 1)
			{
				ref ulong reference = ref _h[num];
				reference ^= config[num];
				num2 = num;
			}
		}

		public void HashCore(byte[] array, int start, int count)
		{
			if (!_isInitialized)
			{
				throw new InvalidOperationException("Not initialized");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (start < 0)
			{
				throw new ArgumentOutOfRangeException("start");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((long)start + (long)count > array.Length)
			{
				throw new ArgumentOutOfRangeException("start+count");
			}
			int num = start;
			int num2 = 128 - _bufferFilled;
			if (_bufferFilled > 0 && count > num2)
			{
				Array.Copy(array, num, _buf, _bufferFilled, num2);
				_counter0 += 128uL;
				if (_counter0 == 0)
				{
					_counter1++;
				}
				Compress(_buf, 0);
				num += num2;
				count -= num2;
				_bufferFilled = 0;
			}
			while (count > 128)
			{
				_counter0 += 128uL;
				if (_counter0 == 0)
				{
					_counter1++;
				}
				Compress(array, num);
				num += 128;
				count -= 128;
			}
			if (count > 0)
			{
				Array.Copy(array, num, _buf, _bufferFilled, count);
				_bufferFilled += count;
			}
		}

		public byte[] HashFinal()
		{
			return HashFinal(isEndOfLayer: false);
		}

		public byte[] HashFinal(bool isEndOfLayer)
		{
			if (!_isInitialized)
			{
				throw new InvalidOperationException("Not initialized");
			}
			_isInitialized = false;
			_counter0 += (uint)_bufferFilled;
			_finalizationFlag0 = ulong.MaxValue;
			if (isEndOfLayer)
			{
				_finalizationFlag1 = ulong.MaxValue;
			}
			int num2;
			for (int num = _bufferFilled; num < _buf.Length; num = num2 + 1)
			{
				_buf[num] = 0;
				num2 = num;
			}
			Compress(_buf, 0);
			byte[] array = new byte[64];
			for (int num3 = 0; num3 < 8; num3 = num2)
			{
				UInt64ToBytes(_h[num3], array, num3 << 3);
				num2 = num3 + 1;
			}
			return array;
		}
	}
}
