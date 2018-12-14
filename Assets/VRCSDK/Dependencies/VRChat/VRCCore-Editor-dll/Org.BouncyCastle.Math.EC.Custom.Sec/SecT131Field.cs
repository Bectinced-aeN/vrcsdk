using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT131Field
	{
		private const ulong M03 = 7uL;

		private const ulong M44 = 17592186044415uL;

		public static void Add(ulong[] x, ulong[] y, ulong[] z)
		{
			z[0] = (x[0] ^ y[0]);
			z[1] = (x[1] ^ y[1]);
			z[2] = (x[2] ^ y[2]);
		}

		public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
		{
			zz[0] = (xx[0] ^ yy[0]);
			zz[1] = (xx[1] ^ yy[1]);
			zz[2] = (xx[2] ^ yy[2]);
			zz[3] = (xx[3] ^ yy[3]);
			zz[4] = (xx[4] ^ yy[4]);
		}

		public static void AddOne(ulong[] x, ulong[] z)
		{
			z[0] = (x[0] ^ 1);
			z[1] = x[1];
			z[2] = x[2];
		}

		public static ulong[] FromBigInteger(BigInteger x)
		{
			ulong[] array = Nat192.FromBigInteger64(x);
			Reduce61(array, 0);
			return array;
		}

		public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
		{
			ulong[] array = Nat192.CreateExt64();
			ImplMultiply(x, y, array);
			Reduce(array, z);
		}

		public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = Nat192.CreateExt64();
			ImplMultiply(x, y, array);
			AddExt(zz, array, zz);
		}

		public static void Reduce(ulong[] xx, ulong[] z)
		{
			ulong num = xx[0];
			ulong num2 = xx[1];
			ulong num3 = xx[2];
			ulong num4 = xx[3];
			ulong num5 = xx[4];
			num2 ^= ((num5 << 61) ^ (num5 << 63));
			num3 ^= ((num5 >> 3) ^ (num5 >> 1) ^ num5 ^ (num5 << 5));
			num4 ^= num5 >> 59;
			num ^= ((num4 << 61) ^ (num4 << 63));
			num2 ^= ((num4 >> 3) ^ (num4 >> 1) ^ num4 ^ (num4 << 5));
			num3 ^= num4 >> 59;
			ulong num6 = num3 >> 3;
			z[0] = (num ^ num6 ^ (num6 << 2) ^ (num6 << 3) ^ (num6 << 8));
			z[1] = (num2 ^ (num6 >> 56));
			z[2] = (num3 & 7);
		}

		public static void Reduce61(ulong[] z, int zOff)
		{
			ulong num = z[zOff + 2];
			ulong num2 = num >> 3;
			z[zOff] ^= (num2 ^ (num2 << 2) ^ (num2 << 3) ^ (num2 << 8));
			z[zOff + 1] ^= num2 >> 56;
			z[zOff + 2] = (num & 7);
		}

		public static void Square(ulong[] x, ulong[] z)
		{
			ulong[] array = Nat.Create64(5);
			ImplSquare(x, array);
			Reduce(array, z);
		}

		public static void SquareAddToExt(ulong[] x, ulong[] zz)
		{
			ulong[] array = Nat.Create64(5);
			ImplSquare(x, array);
			AddExt(zz, array, zz);
		}

		public static void SquareN(ulong[] x, int n, ulong[] z)
		{
			ulong[] array = Nat.Create64(5);
			ImplSquare(x, array);
			Reduce(array, z);
			while (--n > 0)
			{
				ImplSquare(z, array);
				Reduce(array, z);
			}
		}

		protected static void ImplCompactExt(ulong[] zz)
		{
			ulong num = zz[0];
			ulong num2 = zz[1];
			ulong num3 = zz[2];
			ulong num4 = zz[3];
			ulong num5 = zz[4];
			ulong num6 = zz[5];
			zz[0] = (num ^ (num2 << 44));
			zz[1] = ((num2 >> 20) ^ (num3 << 24));
			zz[2] = ((num3 >> 40) ^ (num4 << 4) ^ (num5 << 48));
			zz[3] = ((num4 >> 60) ^ (num6 << 28) ^ (num5 >> 16));
			zz[4] = num6 >> 36;
			zz[5] = 0uL;
		}

		protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong num = x[0];
			ulong num2 = x[1];
			ulong num3 = x[2];
			num3 = (((num2 >> 24) ^ (num3 << 40)) & 0xFFFFFFFFFFF);
			num2 = (((num >> 44) ^ (num2 << 20)) & 0xFFFFFFFFFFF);
			num &= 0xFFFFFFFFFFF;
			ulong num4 = y[0];
			ulong num5 = y[1];
			ulong num6 = y[2];
			num6 = (((num5 >> 24) ^ (num6 << 40)) & 0xFFFFFFFFFFF);
			num5 = (((num4 >> 44) ^ (num5 << 20)) & 0xFFFFFFFFFFF);
			num4 &= 0xFFFFFFFFFFF;
			ulong[] array = new ulong[10];
			ImplMulw(num, num4, array, 0);
			ImplMulw(num3, num6, array, 2);
			ulong num7 = num ^ num2 ^ num3;
			ulong num8 = num4 ^ num5 ^ num6;
			ImplMulw(num7, num8, array, 4);
			ulong num9 = (num2 << 1) ^ (num3 << 2);
			ulong num10 = (num5 << 1) ^ (num6 << 2);
			ImplMulw(num ^ num9, num4 ^ num10, array, 6);
			ImplMulw(num7 ^ num9, num8 ^ num10, array, 8);
			ulong num11 = array[6] ^ array[8];
			ulong num12 = array[7] ^ array[9];
			ulong num13 = (num11 << 1) ^ array[6];
			ulong num14 = num11 ^ (num12 << 1) ^ array[7];
			ulong num15 = num12;
			ulong num16 = array[0];
			ulong num17 = array[1] ^ array[0] ^ array[4];
			ulong num18 = array[1] ^ array[5];
			ulong num19 = num16 ^ num13 ^ (array[2] << 4) ^ (array[2] << 1);
			ulong num20 = num17 ^ num14 ^ (array[3] << 4) ^ (array[3] << 1);
			ulong num21 = num18 ^ num15;
			num20 ^= num19 >> 44;
			num19 &= 0xFFFFFFFFFFF;
			num21 ^= num20 >> 44;
			num20 &= 0xFFFFFFFFFFF;
			num19 = ((num19 >> 1) ^ ((num20 & 1) << 43));
			num20 = ((num20 >> 1) ^ ((num21 & 1) << 43));
			num21 >>= 1;
			num19 ^= num19 << 1;
			num19 ^= num19 << 2;
			num19 ^= num19 << 4;
			num19 ^= num19 << 8;
			num19 ^= num19 << 16;
			num19 ^= num19 << 32;
			num19 &= 0xFFFFFFFFFFF;
			num20 ^= num19 >> 43;
			num20 ^= num20 << 1;
			num20 ^= num20 << 2;
			num20 ^= num20 << 4;
			num20 ^= num20 << 8;
			num20 ^= num20 << 16;
			num20 ^= num20 << 32;
			num20 &= 0xFFFFFFFFFFF;
			num21 ^= num20 >> 43;
			num21 ^= num21 << 1;
			num21 ^= num21 << 2;
			num21 ^= num21 << 4;
			num21 ^= num21 << 8;
			num21 ^= num21 << 16;
			num21 ^= num21 << 32;
			zz[0] = num16;
			zz[1] = (num17 ^ num19 ^ array[2]);
			zz[2] = (num18 ^ num20 ^ num19 ^ array[3]);
			zz[3] = (num21 ^ num20);
			zz[4] = (num21 ^ array[2]);
			zz[5] = array[3];
			ImplCompactExt(zz);
		}

		protected static void ImplMulw(ulong x, ulong y, ulong[] z, int zOff)
		{
			ulong[] array = new ulong[8]
			{
				0uL,
				y,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL,
				0uL
			};
			array[2] = array[1] << 1;
			array[3] = (array[2] ^ y);
			array[4] = array[2] << 1;
			array[5] = (array[4] ^ y);
			array[6] = array[3] << 1;
			array[7] = (array[6] ^ y);
			uint num = (uint)x;
			ulong num2 = 0uL;
			ulong num3 = array[num & 7] ^ (array[(num >> 3) & 7] << 3) ^ (array[(num >> 6) & 7] << 6);
			int num4 = 33;
			do
			{
				num = (uint)(x >> num4);
				ulong num5 = array[num & 7] ^ (array[(num >> 3) & 7] << 3) ^ (array[(num >> 6) & 7] << 6) ^ (array[(num >> 9) & 7] << 9);
				num3 ^= num5 << (num4 & 0x3F);
				num2 ^= num5 >> (-num4 & 0x3F);
			}
			while ((num4 -= 12) > 0);
			z[zOff] = (num3 & 0xFFFFFFFFFFF);
			z[zOff + 1] = ((num3 >> 44) ^ (num2 << 20));
		}

		protected static void ImplSquare(ulong[] x, ulong[] zz)
		{
			Interleave.Expand64To128(x[0], zz, 0);
			Interleave.Expand64To128(x[1], zz, 2);
			zz[4] = Interleave.Expand8to16((uint)x[2]);
		}
	}
}
