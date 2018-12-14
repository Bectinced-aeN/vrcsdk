using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	internal abstract class GcmUtilities
	{
		private const uint E1 = 3774873600u;

		private const ulong E1L = 16212958658533785600uL;

		private static readonly uint[] LOOKUP = GenerateLookup();

		private static uint[] GenerateLookup()
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = 0u;
				for (int num2 = 7; num2 >= 0; num2--)
				{
					if ((i & (1 << num2)) != 0)
					{
						num ^= 3774873600u >> ((7 - num2) & 0x1F);
					}
				}
				array[i] = num;
			}
			return array;
		}

		internal static byte[] OneAsBytes()
		{
			return new byte[16]
			{
				128,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
		}

		internal static uint[] OneAsUints()
		{
			return new uint[4]
			{
				2147483648u,
				0u,
				0u,
				0u
			};
		}

		internal static byte[] AsBytes(uint[] x)
		{
			return Pack.UInt32_To_BE(x);
		}

		internal static void AsBytes(uint[] x, byte[] z)
		{
			Pack.UInt32_To_BE(x, z, 0);
		}

		internal static uint[] AsUints(byte[] bs)
		{
			uint[] array = new uint[4];
			Pack.BE_To_UInt32(bs, 0, array);
			return array;
		}

		internal static void AsUints(byte[] bs, uint[] output)
		{
			Pack.BE_To_UInt32(bs, 0, output);
		}

		internal static void Multiply(byte[] x, byte[] y)
		{
			uint[] x2 = AsUints(x);
			uint[] y2 = AsUints(y);
			Multiply(x2, y2);
			AsBytes(x2, x);
		}

		internal static void Multiply(uint[] x, uint[] y)
		{
			uint num = x[0];
			uint num2 = x[1];
			uint num3 = x[2];
			uint num4 = x[3];
			uint num5 = 0u;
			uint num6 = 0u;
			uint num7 = 0u;
			uint num8 = 0u;
			for (int i = 0; i < 4; i++)
			{
				int num9 = (int)y[i];
				for (int j = 0; j < 32; j++)
				{
					uint num10 = (uint)(num9 >> 31);
					num9 <<= 1;
					num5 ^= (num & num10);
					num6 ^= (num2 & num10);
					num7 ^= (num3 & num10);
					num8 ^= (num4 & num10);
					uint num11 = (uint)((int)(num4 << 31) >> 8);
					num4 = ((num4 >> 1) | (num3 << 31));
					num3 = ((num3 >> 1) | (num2 << 31));
					num2 = ((num2 >> 1) | (num << 31));
					num = (uint)((int)(num >> 1) ^ ((int)num11 & -520093696));
				}
			}
			x[0] = num5;
			x[1] = num6;
			x[2] = num7;
			x[3] = num8;
		}

		internal static void Multiply(ulong[] x, ulong[] y)
		{
			ulong num = x[0];
			ulong num2 = x[1];
			ulong num3 = 0uL;
			ulong num4 = 0uL;
			for (int i = 0; i < 2; i++)
			{
				long num5 = (long)y[i];
				for (int j = 0; j < 64; j++)
				{
					ulong num6 = (ulong)(num5 >> 63);
					num5 <<= 1;
					num3 ^= (num & num6);
					num4 ^= (num2 & num6);
					ulong num7 = num2 << 63 >> 8;
					num2 = ((num2 >> 1) | (num << 63));
					num = (ulong)((long)(num >> 1) ^ ((long)num7 & -2233785415175766016L));
				}
			}
			x[0] = num3;
			x[1] = num4;
		}

		internal static void MultiplyP(uint[] x)
		{
			uint num = (uint)((int)ShiftRight(x) >> 8);
			x[0] ^= (uint)((int)num & -520093696);
		}

		internal static void MultiplyP(uint[] x, uint[] z)
		{
			uint num = (uint)((int)ShiftRight(x, z) >> 8);
			z[0] ^= (uint)((int)num & -520093696);
		}

		internal static void MultiplyP8(uint[] x)
		{
			uint num = ShiftRightN(x, 8);
			x[0] ^= LOOKUP[num >> 24];
		}

		internal static void MultiplyP8(uint[] x, uint[] y)
		{
			uint num = ShiftRightN(x, 8, y);
			y[0] ^= LOOKUP[num >> 24];
		}

		internal static uint ShiftRight(uint[] x)
		{
			uint num = x[0];
			x[0] = num >> 1;
			uint num2 = num << 31;
			num = x[1];
			x[1] = ((num >> 1) | num2);
			num2 = num << 31;
			num = x[2];
			x[2] = ((num >> 1) | num2);
			num2 = num << 31;
			num = x[3];
			x[3] = ((num >> 1) | num2);
			return num << 31;
		}

		internal static uint ShiftRight(uint[] x, uint[] z)
		{
			uint num = x[0];
			z[0] = num >> 1;
			uint num2 = num << 31;
			num = x[1];
			z[1] = ((num >> 1) | num2);
			num2 = num << 31;
			num = x[2];
			z[2] = ((num >> 1) | num2);
			num2 = num << 31;
			num = x[3];
			z[3] = ((num >> 1) | num2);
			return num << 31;
		}

		internal static uint ShiftRightN(uint[] x, int n)
		{
			uint num = x[0];
			int num2 = 32 - n;
			x[0] = num >> n;
			uint num3 = num << num2;
			num = x[1];
			x[1] = ((num >> n) | num3);
			num3 = num << num2;
			num = x[2];
			x[2] = ((num >> n) | num3);
			num3 = num << num2;
			num = x[3];
			x[3] = ((num >> n) | num3);
			return num << num2;
		}

		internal static uint ShiftRightN(uint[] x, int n, uint[] z)
		{
			uint num = x[0];
			int num2 = 32 - n;
			z[0] = num >> n;
			uint num3 = num << num2;
			num = x[1];
			z[1] = ((num >> n) | num3);
			num3 = num << num2;
			num = x[2];
			z[2] = ((num >> n) | num3);
			num3 = num << num2;
			num = x[3];
			z[3] = ((num >> n) | num3);
			return num << num2;
		}

		internal static void Xor(byte[] x, byte[] y)
		{
			int num = 0;
			do
			{
				x[num] ^= y[num];
				num++;
				x[num] ^= y[num];
				num++;
				x[num] ^= y[num];
				num++;
				x[num] ^= y[num];
				num++;
			}
			while (num < 16);
		}

		internal static void Xor(byte[] x, byte[] y, int yOff, int yLen)
		{
			while (--yLen >= 0)
			{
				x[yLen] ^= y[yOff + yLen];
			}
		}

		internal static void Xor(byte[] x, byte[] y, byte[] z)
		{
			int num = 0;
			do
			{
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
			}
			while (num < 16);
		}

		internal static void Xor(uint[] x, uint[] y)
		{
			x[0] ^= y[0];
			x[1] ^= y[1];
			x[2] ^= y[2];
			x[3] ^= y[3];
		}

		internal static void Xor(uint[] x, uint[] y, uint[] z)
		{
			z[0] = (x[0] ^ y[0]);
			z[1] = (x[1] ^ y[1]);
			z[2] = (x[2] ^ y[2]);
			z[3] = (x[3] ^ y[3]);
		}
	}
}
