using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Math.Raw
{
	internal abstract class Nat128
	{
		private const ulong M = 4294967295uL;

		public static uint Add(uint[] x, uint[] y, uint[] z)
		{
			ulong num = 0uL;
			num = (ulong)((long)num + ((long)x[0] + (long)y[0]));
			z[0] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[1] + (long)y[1]));
			z[1] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[2] + (long)y[2]));
			z[2] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[3] + (long)y[3]));
			z[3] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		public static uint AddBothTo(uint[] x, uint[] y, uint[] z)
		{
			ulong num = 0uL;
			num = (ulong)((long)num + ((long)x[0] + (long)y[0] + z[0]));
			z[0] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[1] + (long)y[1] + z[1]));
			z[1] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[2] + (long)y[2] + z[2]));
			z[2] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[3] + (long)y[3] + z[3]));
			z[3] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		public static uint AddTo(uint[] x, uint[] z)
		{
			ulong num = 0uL;
			num = (ulong)((long)num + ((long)x[0] + (long)z[0]));
			z[0] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[1] + (long)z[1]));
			z[1] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[2] + (long)z[2]));
			z[2] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[3] + (long)z[3]));
			z[3] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		public static uint AddTo(uint[] x, int xOff, uint[] z, int zOff, uint cIn)
		{
			ulong num = cIn;
			num = (ulong)((long)num + ((long)x[xOff] + (long)z[zOff]));
			z[zOff] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[xOff + 1] + (long)z[zOff + 1]));
			z[zOff + 1] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[xOff + 2] + (long)z[zOff + 2]));
			z[zOff + 2] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)x[xOff + 3] + (long)z[zOff + 3]));
			z[zOff + 3] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		public static uint AddToEachOther(uint[] u, int uOff, uint[] v, int vOff)
		{
			ulong num = 0uL;
			num = (ulong)((long)num + ((long)u[uOff] + (long)v[vOff]));
			u[uOff] = (uint)num;
			v[vOff] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)u[uOff + 1] + (long)v[vOff + 1]));
			u[uOff + 1] = (uint)num;
			v[vOff + 1] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)u[uOff + 2] + (long)v[vOff + 2]));
			u[uOff + 2] = (uint)num;
			v[vOff + 2] = (uint)num;
			num >>= 32;
			num = (ulong)((long)num + ((long)u[uOff + 3] + (long)v[vOff + 3]));
			u[uOff + 3] = (uint)num;
			v[vOff + 3] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		public static void Copy(uint[] x, uint[] z)
		{
			z[0] = x[0];
			z[1] = x[1];
			z[2] = x[2];
			z[3] = x[3];
		}

		public static void Copy64(ulong[] x, ulong[] z)
		{
			z[0] = x[0];
			z[1] = x[1];
		}

		public static uint[] Create()
		{
			return new uint[4];
		}

		public static ulong[] Create64()
		{
			return new ulong[2];
		}

		public static uint[] CreateExt()
		{
			return new uint[8];
		}

		public static ulong[] CreateExt64()
		{
			return new ulong[4];
		}

		public static bool Diff(uint[] x, int xOff, uint[] y, int yOff, uint[] z, int zOff)
		{
			bool flag = Gte(x, xOff, y, yOff);
			if (flag)
			{
				Sub(x, xOff, y, yOff, z, zOff);
			}
			else
			{
				Sub(y, yOff, x, xOff, z, zOff);
			}
			return flag;
		}

		public static bool Eq(uint[] x, uint[] y)
		{
			for (int num = 3; num >= 0; num--)
			{
				if (x[num] != y[num])
				{
					return false;
				}
			}
			return true;
		}

		public static bool Eq64(ulong[] x, ulong[] y)
		{
			for (int num = 1; num >= 0; num--)
			{
				if (x[num] != y[num])
				{
					return false;
				}
			}
			return true;
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			if (x.SignValue < 0 || x.BitLength > 128)
			{
				throw new ArgumentException();
			}
			uint[] array = Create();
			int num = 0;
			while (x.SignValue != 0)
			{
				array[num++] = (uint)x.IntValue;
				x = x.ShiftRight(32);
			}
			return array;
		}

		public static ulong[] FromBigInteger64(BigInteger x)
		{
			if (x.SignValue < 0 || x.BitLength > 128)
			{
				throw new ArgumentException();
			}
			ulong[] array = Create64();
			int num = 0;
			while (x.SignValue != 0)
			{
				array[num++] = (ulong)x.LongValue;
				x = x.ShiftRight(64);
			}
			return array;
		}

		public static uint GetBit(uint[] x, int bit)
		{
			if (bit == 0)
			{
				return x[0] & 1;
			}
			if ((bit & 0x7F) != bit)
			{
				return 0u;
			}
			int num = bit >> 5;
			int num2 = bit & 0x1F;
			return (x[num] >> num2) & 1;
		}

		public static bool Gte(uint[] x, uint[] y)
		{
			for (int num = 3; num >= 0; num--)
			{
				uint num2 = x[num];
				uint num3 = y[num];
				if (num2 < num3)
				{
					return false;
				}
				if (num2 > num3)
				{
					return true;
				}
			}
			return true;
		}

		public static bool Gte(uint[] x, int xOff, uint[] y, int yOff)
		{
			for (int num = 3; num >= 0; num--)
			{
				uint num2 = x[xOff + num];
				uint num3 = y[yOff + num];
				if (num2 < num3)
				{
					return false;
				}
				if (num2 > num3)
				{
					return true;
				}
			}
			return true;
		}

		public static bool IsOne(uint[] x)
		{
			if (x[0] != 1)
			{
				return false;
			}
			for (int i = 1; i < 4; i++)
			{
				if (x[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsOne64(ulong[] x)
		{
			if (x[0] != 1)
			{
				return false;
			}
			for (int i = 1; i < 2; i++)
			{
				if (x[i] != 0L)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsZero(uint[] x)
		{
			for (int i = 0; i < 4; i++)
			{
				if (x[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsZero64(ulong[] x)
		{
			for (int i = 0; i < 2; i++)
			{
				if (x[i] != 0L)
				{
					return false;
				}
			}
			return true;
		}

		public static void Mul(uint[] x, uint[] y, uint[] zz)
		{
			ulong num = y[0];
			ulong num2 = y[1];
			ulong num3 = y[2];
			ulong num4 = y[3];
			ulong num5 = 0uL;
			ulong num6 = x[0];
			num5 += num6 * num;
			zz[0] = (uint)num5;
			num5 >>= 32;
			num5 += num6 * num2;
			zz[1] = (uint)num5;
			num5 >>= 32;
			num5 += num6 * num3;
			zz[2] = (uint)num5;
			num5 >>= 32;
			num5 += num6 * num4;
			zz[3] = (uint)num5;
			num5 >>= 32;
			zz[4] = (uint)num5;
			for (int i = 1; i < 4; i++)
			{
				ulong num7 = 0uL;
				ulong num8 = x[i];
				num7 += num8 * num + zz[i];
				zz[i] = (uint)num7;
				num7 >>= 32;
				num7 += num8 * num2 + zz[i + 1];
				zz[i + 1] = (uint)num7;
				num7 >>= 32;
				num7 += num8 * num3 + zz[i + 2];
				zz[i + 2] = (uint)num7;
				num7 >>= 32;
				num7 += num8 * num4 + zz[i + 3];
				zz[i + 3] = (uint)num7;
				num7 >>= 32;
				zz[i + 4] = (uint)num7;
			}
		}

		public static void Mul(uint[] x, int xOff, uint[] y, int yOff, uint[] zz, int zzOff)
		{
			ulong num = y[yOff];
			ulong num2 = y[yOff + 1];
			ulong num3 = y[yOff + 2];
			ulong num4 = y[yOff + 3];
			ulong num5 = 0uL;
			ulong num6 = x[xOff];
			num5 += num6 * num;
			zz[zzOff] = (uint)num5;
			num5 >>= 32;
			num5 += num6 * num2;
			zz[zzOff + 1] = (uint)num5;
			num5 >>= 32;
			num5 += num6 * num3;
			zz[zzOff + 2] = (uint)num5;
			num5 >>= 32;
			num5 += num6 * num4;
			zz[zzOff + 3] = (uint)num5;
			num5 >>= 32;
			zz[zzOff + 4] = (uint)num5;
			for (int i = 1; i < 4; i++)
			{
				zzOff++;
				ulong num7 = 0uL;
				ulong num8 = x[xOff + i];
				num7 += num8 * num + zz[zzOff];
				zz[zzOff] = (uint)num7;
				num7 >>= 32;
				num7 += num8 * num2 + zz[zzOff + 1];
				zz[zzOff + 1] = (uint)num7;
				num7 >>= 32;
				num7 += num8 * num3 + zz[zzOff + 2];
				zz[zzOff + 2] = (uint)num7;
				num7 >>= 32;
				num7 += num8 * num4 + zz[zzOff + 3];
				zz[zzOff + 3] = (uint)num7;
				num7 >>= 32;
				zz[zzOff + 4] = (uint)num7;
			}
		}

		public static uint MulAddTo(uint[] x, uint[] y, uint[] zz)
		{
			ulong num = y[0];
			ulong num2 = y[1];
			ulong num3 = y[2];
			ulong num4 = y[3];
			ulong num5 = 0uL;
			for (int i = 0; i < 4; i++)
			{
				ulong num6 = 0uL;
				ulong num7 = x[i];
				num6 += num7 * num + zz[i];
				zz[i] = (uint)num6;
				num6 >>= 32;
				num6 += num7 * num2 + zz[i + 1];
				zz[i + 1] = (uint)num6;
				num6 >>= 32;
				num6 += num7 * num3 + zz[i + 2];
				zz[i + 2] = (uint)num6;
				num6 >>= 32;
				num6 += num7 * num4 + zz[i + 3];
				zz[i + 3] = (uint)num6;
				num6 >>= 32;
				num6 += num5 + zz[i + 4];
				zz[i + 4] = (uint)num6;
				num5 = num6 >> 32;
			}
			return (uint)num5;
		}

		public static uint MulAddTo(uint[] x, int xOff, uint[] y, int yOff, uint[] zz, int zzOff)
		{
			ulong num = y[yOff];
			ulong num2 = y[yOff + 1];
			ulong num3 = y[yOff + 2];
			ulong num4 = y[yOff + 3];
			ulong num5 = 0uL;
			for (int i = 0; i < 4; i++)
			{
				ulong num6 = 0uL;
				ulong num7 = x[xOff + i];
				num6 += num7 * num + zz[zzOff];
				zz[zzOff] = (uint)num6;
				num6 >>= 32;
				num6 += num7 * num2 + zz[zzOff + 1];
				zz[zzOff + 1] = (uint)num6;
				num6 >>= 32;
				num6 += num7 * num3 + zz[zzOff + 2];
				zz[zzOff + 2] = (uint)num6;
				num6 >>= 32;
				num6 += num7 * num4 + zz[zzOff + 3];
				zz[zzOff + 3] = (uint)num6;
				num6 >>= 32;
				num6 += num5 + zz[zzOff + 4];
				zz[zzOff + 4] = (uint)num6;
				num5 = num6 >> 32;
				zzOff++;
			}
			return (uint)num5;
		}

		public static ulong Mul33Add(uint w, uint[] x, int xOff, uint[] y, int yOff, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = w;
			ulong num3 = x[xOff];
			num += num2 * num3 + y[yOff];
			z[zOff] = (uint)num;
			num >>= 32;
			ulong num4 = x[xOff + 1];
			num += num2 * num4 + num3 + y[yOff + 1];
			z[zOff + 1] = (uint)num;
			num >>= 32;
			ulong num5 = x[xOff + 2];
			num += num2 * num5 + num4 + y[yOff + 2];
			z[zOff + 2] = (uint)num;
			num >>= 32;
			ulong num6 = x[xOff + 3];
			num += num2 * num6 + num5 + y[yOff + 3];
			z[zOff + 3] = (uint)num;
			num >>= 32;
			return num + num6;
		}

		public static uint MulWordAddExt(uint x, uint[] yy, int yyOff, uint[] zz, int zzOff)
		{
			ulong num = 0uL;
			ulong num2 = x;
			num += num2 * yy[yyOff] + zz[zzOff];
			zz[zzOff] = (uint)num;
			num >>= 32;
			num += num2 * yy[yyOff + 1] + zz[zzOff + 1];
			zz[zzOff + 1] = (uint)num;
			num >>= 32;
			num += num2 * yy[yyOff + 2] + zz[zzOff + 2];
			zz[zzOff + 2] = (uint)num;
			num >>= 32;
			num += num2 * yy[yyOff + 3] + zz[zzOff + 3];
			zz[zzOff + 3] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		public static uint Mul33DWordAdd(uint x, ulong y, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = x;
			ulong num3 = y & uint.MaxValue;
			num += num2 * num3 + z[zOff];
			z[zOff] = (uint)num;
			num >>= 32;
			ulong num4 = y >> 32;
			num += num2 * num4 + num3 + z[zOff + 1];
			z[zOff + 1] = (uint)num;
			num >>= 32;
			num += num4 + z[zOff + 2];
			z[zOff + 2] = (uint)num;
			num >>= 32;
			num += z[zOff + 3];
			z[zOff + 3] = (uint)num;
			num >>= 32;
			return (uint)num;
		}

		public static uint Mul33WordAdd(uint x, uint y, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = y;
			num += num2 * x + z[zOff];
			z[zOff] = (uint)num;
			num >>= 32;
			num += num2 + z[zOff + 1];
			z[zOff + 1] = (uint)num;
			num >>= 32;
			num += z[zOff + 2];
			z[zOff + 2] = (uint)num;
			num >>= 32;
			return (num != 0L) ? Nat.IncAt(4, z, zOff, 3) : 0;
		}

		public static uint MulWordDwordAdd(uint x, ulong y, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = x;
			num += num2 * y + z[zOff];
			z[zOff] = (uint)num;
			num >>= 32;
			num += num2 * (y >> 32) + z[zOff + 1];
			z[zOff + 1] = (uint)num;
			num >>= 32;
			num += z[zOff + 2];
			z[zOff + 2] = (uint)num;
			num >>= 32;
			return (num != 0L) ? Nat.IncAt(4, z, zOff, 3) : 0;
		}

		public static uint MulWordsAdd(uint x, uint y, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = x;
			ulong num3 = y;
			num += num3 * num2 + z[zOff];
			z[zOff] = (uint)num;
			num >>= 32;
			num += z[zOff + 1];
			z[zOff + 1] = (uint)num;
			num >>= 32;
			return (num != 0L) ? Nat.IncAt(4, z, zOff, 2) : 0;
		}

		public static uint MulWord(uint x, uint[] y, uint[] z, int zOff)
		{
			ulong num = 0uL;
			ulong num2 = x;
			int num3 = 0;
			do
			{
				num += num2 * y[num3];
				z[zOff + num3] = (uint)num;
				num >>= 32;
			}
			while (++num3 < 4);
			return (uint)num;
		}

		public static void Square(uint[] x, uint[] zz)
		{
			ulong num = x[0];
			uint num2 = 0u;
			int num3 = 3;
			int num4 = 8;
			do
			{
				ulong num6 = x[num3--];
				ulong num7 = num6 * num6;
				zz[--num4] = (uint)((int)(num2 << 31) | (int)(num7 >> 33));
				zz[--num4] = (uint)(num7 >> 1);
				num2 = (uint)num7;
			}
			while (num3 > 0);
			ulong num8 = num * num;
			ulong num9 = (num2 << 31) | (num8 >> 33);
			zz[0] = (uint)num8;
			num2 = (uint)((int)(num8 >> 32) & 1);
			ulong num10 = x[1];
			ulong num11 = zz[2];
			num9 += num10 * num;
			uint num12 = (uint)num9;
			zz[1] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num11 += num9 >> 32;
			ulong num13 = x[2];
			ulong num14 = zz[3];
			ulong num15 = zz[4];
			num11 += num13 * num;
			num12 = (uint)num11;
			zz[2] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num14 += (num11 >> 32) + num13 * num10;
			num15 += num14 >> 32;
			num14 &= uint.MaxValue;
			ulong num16 = x[3];
			ulong num17 = zz[5];
			ulong num18 = zz[6];
			num14 += num16 * num;
			num12 = (uint)num14;
			zz[3] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num15 += (num14 >> 32) + num16 * num10;
			num17 += (num15 >> 32) + num16 * num13;
			num18 += num17 >> 32;
			num12 = (uint)num15;
			zz[4] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num12 = (uint)num17;
			zz[5] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num12 = (uint)num18;
			zz[6] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num12 = (uint)((int)zz[7] + (int)(num18 >> 32));
			zz[7] = ((num12 << 1) | num2);
		}

		public static void Square(uint[] x, int xOff, uint[] zz, int zzOff)
		{
			ulong num = x[xOff];
			uint num2 = 0u;
			int num3 = 3;
			int num4 = 8;
			do
			{
				ulong num6 = x[xOff + num3--];
				ulong num7 = num6 * num6;
				zz[zzOff + --num4] = (uint)((int)(num2 << 31) | (int)(num7 >> 33));
				zz[zzOff + --num4] = (uint)(num7 >> 1);
				num2 = (uint)num7;
			}
			while (num3 > 0);
			ulong num8 = num * num;
			ulong num9 = (num2 << 31) | (num8 >> 33);
			zz[zzOff] = (uint)num8;
			num2 = (uint)((int)(num8 >> 32) & 1);
			ulong num10 = x[xOff + 1];
			ulong num11 = zz[zzOff + 2];
			num9 += num10 * num;
			uint num12 = (uint)num9;
			zz[zzOff + 1] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num11 += num9 >> 32;
			ulong num13 = x[xOff + 2];
			ulong num14 = zz[zzOff + 3];
			ulong num15 = zz[zzOff + 4];
			num11 += num13 * num;
			num12 = (uint)num11;
			zz[zzOff + 2] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num14 += (num11 >> 32) + num13 * num10;
			num15 += num14 >> 32;
			num14 &= uint.MaxValue;
			ulong num16 = x[xOff + 3];
			ulong num17 = zz[zzOff + 5];
			ulong num18 = zz[zzOff + 6];
			num14 += num16 * num;
			num12 = (uint)num14;
			zz[zzOff + 3] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num15 += (num14 >> 32) + num16 * num10;
			num17 += (num15 >> 32) + num16 * num13;
			num18 += num17 >> 32;
			num12 = (uint)num15;
			zz[zzOff + 4] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num12 = (uint)num17;
			zz[zzOff + 5] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num12 = (uint)num18;
			zz[zzOff + 6] = ((num12 << 1) | num2);
			num2 = num12 >> 31;
			num12 = (uint)((int)zz[zzOff + 7] + (int)(num18 >> 32));
			zz[zzOff + 7] = ((num12 << 1) | num2);
		}

		public static int Sub(uint[] x, uint[] y, uint[] z)
		{
			long num = 0L;
			num += (long)x[0] - (long)y[0];
			z[0] = (uint)num;
			num >>= 32;
			num += (long)x[1] - (long)y[1];
			z[1] = (uint)num;
			num >>= 32;
			num += (long)x[2] - (long)y[2];
			z[2] = (uint)num;
			num >>= 32;
			num += (long)x[3] - (long)y[3];
			z[3] = (uint)num;
			num >>= 32;
			return (int)num;
		}

		public static int Sub(uint[] x, int xOff, uint[] y, int yOff, uint[] z, int zOff)
		{
			long num = 0L;
			num += (long)x[xOff] - (long)y[yOff];
			z[zOff] = (uint)num;
			num >>= 32;
			num += (long)x[xOff + 1] - (long)y[yOff + 1];
			z[zOff + 1] = (uint)num;
			num >>= 32;
			num += (long)x[xOff + 2] - (long)y[yOff + 2];
			z[zOff + 2] = (uint)num;
			num >>= 32;
			num += (long)x[xOff + 3] - (long)y[yOff + 3];
			z[zOff + 3] = (uint)num;
			num >>= 32;
			return (int)num;
		}

		public static int SubBothFrom(uint[] x, uint[] y, uint[] z)
		{
			long num = 0L;
			num += (long)z[0] - (long)x[0] - y[0];
			z[0] = (uint)num;
			num >>= 32;
			num += (long)z[1] - (long)x[1] - y[1];
			z[1] = (uint)num;
			num >>= 32;
			num += (long)z[2] - (long)x[2] - y[2];
			z[2] = (uint)num;
			num >>= 32;
			num += (long)z[3] - (long)x[3] - y[3];
			z[3] = (uint)num;
			num >>= 32;
			return (int)num;
		}

		public static int SubFrom(uint[] x, uint[] z)
		{
			long num = 0L;
			num += (long)z[0] - (long)x[0];
			z[0] = (uint)num;
			num >>= 32;
			num += (long)z[1] - (long)x[1];
			z[1] = (uint)num;
			num >>= 32;
			num += (long)z[2] - (long)x[2];
			z[2] = (uint)num;
			num >>= 32;
			num += (long)z[3] - (long)x[3];
			z[3] = (uint)num;
			num >>= 32;
			return (int)num;
		}

		public static int SubFrom(uint[] x, int xOff, uint[] z, int zOff)
		{
			long num = 0L;
			num += (long)z[zOff] - (long)x[xOff];
			z[zOff] = (uint)num;
			num >>= 32;
			num += (long)z[zOff + 1] - (long)x[xOff + 1];
			z[zOff + 1] = (uint)num;
			num >>= 32;
			num += (long)z[zOff + 2] - (long)x[xOff + 2];
			z[zOff + 2] = (uint)num;
			num >>= 32;
			num += (long)z[zOff + 3] - (long)x[xOff + 3];
			z[zOff + 3] = (uint)num;
			num >>= 32;
			return (int)num;
		}

		public static BigInteger ToBigInteger(uint[] x)
		{
			byte[] array = new byte[16];
			for (int i = 0; i < 4; i++)
			{
				uint num = x[i];
				if (num != 0)
				{
					Pack.UInt32_To_BE(num, array, 3 - i << 2);
				}
			}
			return new BigInteger(1, array);
		}

		public static BigInteger ToBigInteger64(ulong[] x)
		{
			byte[] array = new byte[16];
			for (int i = 0; i < 2; i++)
			{
				ulong num = x[i];
				if (num != 0L)
				{
					Pack.UInt64_To_BE(num, array, 1 - i << 3);
				}
			}
			return new BigInteger(1, array);
		}

		public static void Zero(uint[] z)
		{
			z[0] = 0u;
			z[1] = 0u;
			z[2] = 0u;
			z[3] = 0u;
		}
	}
}
