using Org.BouncyCastle.Math.Raw;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT571Field
	{
		private const ulong M59 = 576460752303423487uL;

		private const ulong RM = 17256631552825064414uL;

		public static void Add(ulong[] x, ulong[] y, ulong[] z)
		{
			for (int i = 0; i < 9; i++)
			{
				z[i] = (x[i] ^ y[i]);
			}
		}

		private static void Add(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
		{
			for (int i = 0; i < 9; i++)
			{
				z[zOff + i] = (x[xOff + i] ^ y[yOff + i]);
			}
		}

		private static void AddBothTo(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
		{
			for (int i = 0; i < 9; i++)
			{
				z[zOff + i] ^= (x[xOff + i] ^ y[yOff + i]);
			}
		}

		public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
		{
			for (int i = 0; i < 18; i++)
			{
				zz[i] = (xx[i] ^ yy[i]);
			}
		}

		public static void AddOne(ulong[] x, ulong[] z)
		{
			z[0] = (x[0] ^ 1);
			for (int i = 1; i < 9; i++)
			{
				z[i] = x[i];
			}
		}

		public static ulong[] FromBigInteger(BigInteger x)
		{
			ulong[] array = Nat576.FromBigInteger64(x);
			Reduce5(array, 0);
			return array;
		}

		public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplMultiply(x, y, array);
			Reduce(array, z);
		}

		public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplMultiply(x, y, array);
			AddExt(zz, array, zz);
		}

		public static void Reduce(ulong[] xx, ulong[] z)
		{
			ulong num = xx[9];
			ulong num2 = xx[17];
			ulong num3 = num;
			num = (num3 ^ (num2 >> 59) ^ (num2 >> 57) ^ (num2 >> 54) ^ (num2 >> 49));
			num3 = (xx[8] ^ (num2 << 5) ^ (num2 << 7) ^ (num2 << 10) ^ (num2 << 15));
			for (int num4 = 16; num4 >= 10; num4--)
			{
				num2 = xx[num4];
				z[num4 - 8] = (num3 ^ (num2 >> 59) ^ (num2 >> 57) ^ (num2 >> 54) ^ (num2 >> 49));
				num3 = (xx[num4 - 9] ^ (num2 << 5) ^ (num2 << 7) ^ (num2 << 10) ^ (num2 << 15));
			}
			num2 = num;
			z[1] = (num3 ^ (num2 >> 59) ^ (num2 >> 57) ^ (num2 >> 54) ^ (num2 >> 49));
			num3 = (xx[0] ^ (num2 << 5) ^ (num2 << 7) ^ (num2 << 10) ^ (num2 << 15));
			ulong num5 = z[8];
			ulong num6 = num5 >> 59;
			z[0] = (num3 ^ num6 ^ (num6 << 2) ^ (num6 << 5) ^ (num6 << 10));
			z[8] = (num5 & 0x7FFFFFFFFFFFFFF);
		}

		public static void Reduce5(ulong[] z, int zOff)
		{
			ulong num = z[zOff + 8];
			ulong num2 = num >> 59;
			z[zOff] ^= (num2 ^ (num2 << 2) ^ (num2 << 5) ^ (num2 << 10));
			z[zOff + 8] = (num & 0x7FFFFFFFFFFFFFF);
		}

		public static void Square(ulong[] x, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplSquare(x, array);
			Reduce(array, z);
		}

		public static void SquareAddToExt(ulong[] x, ulong[] zz)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplSquare(x, array);
			AddExt(zz, array, zz);
		}

		public static void SquareN(ulong[] x, int n, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplSquare(x, array);
			Reduce(array, z);
			while (--n > 0)
			{
				ImplSquare(z, array);
				Reduce(array, z);
			}
		}

		protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = new ulong[144];
			Array.Copy(y, 0, array, 9, 9);
			int num = 0;
			for (int num2 = 7; num2 > 0; num2--)
			{
				num += 18;
				Nat.ShiftUpBit64(9, array, num >> 1, 0uL, array, num);
				Reduce5(array, num);
				Add(array, 9, array, num, array, num + 9);
			}
			ulong[] array2 = new ulong[array.Length];
			Nat.ShiftUpBits64(array.Length, array, 0, 4, 0uL, array2, 0);
			uint num3 = 15u;
			for (int num4 = 56; num4 >= 0; num4 -= 8)
			{
				for (int i = 1; i < 9; i += 2)
				{
					uint num5 = (uint)(x[i] >> num4);
					uint num6 = num5 & num3;
					uint num7 = (num5 >> 4) & num3;
					AddBothTo(array, (int)(9 * num6), array2, (int)(9 * num7), zz, i - 1);
				}
				Nat.ShiftUpBits64(16, zz, 0, 8, 0uL);
			}
			for (int num8 = 56; num8 >= 0; num8 -= 8)
			{
				for (int j = 0; j < 9; j += 2)
				{
					uint num9 = (uint)(x[j] >> num8);
					uint num10 = num9 & num3;
					uint num11 = (num9 >> 4) & num3;
					AddBothTo(array, (int)(9 * num10), array2, (int)(9 * num11), zz, j);
				}
				if (num8 > 0)
				{
					Nat.ShiftUpBits64(18, zz, 0, 8, 0uL);
				}
			}
		}

		protected static void ImplMulwAcc(ulong[] xs, ulong y, ulong[] z, int zOff)
		{
			ulong[] array = new ulong[32]
			{
				0uL,
				y,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL
			};
			for (int i = 2; i < 32; i += 2)
			{
				array[i] = array[i >> 1] << 1;
				array[i + 1] = (array[i] ^ y);
			}
			ulong num = 0uL;
			for (int j = 0; j < 9; j++)
			{
				ulong num2 = xs[j];
				uint num3 = (uint)num2;
				num ^= array[num3 & 0x1F];
				ulong num4 = 0uL;
				int num5 = 60;
				do
				{
					num3 = (uint)(num2 >> num5);
					ulong num6 = array[num3 & 0x1F];
					num ^= num6 << (num5 & 0x3F);
					num4 ^= num6 >> (-num5 & 0x3F);
				}
				while ((num5 -= 5) > 0);
				for (int k = 0; k < 4; k++)
				{
					num2 = (ulong)((long)num2 & -1190112520884487202L) >> 1;
					num4 = (ulong)((long)num4 ^ ((long)num2 & ((long)(y << k) >> 63)));
				}
				z[zOff + j] ^= num;
				num = num4;
			}
			z[zOff + 9] ^= num;
		}

		protected static void ImplSquare(ulong[] x, ulong[] zz)
		{
			for (int i = 0; i < 9; i++)
			{
				Interleave.Expand64To128(x[i], zz, i << 1);
			}
		}
	}
}
