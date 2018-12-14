using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	internal class SerpentEngine : IBlockCipher
	{
		private const int BLOCK_SIZE = 16;

		private static readonly int ROUNDS = 32;

		private static readonly int PHI = -1640531527;

		private bool encrypting;

		private int[] wKey;

		private int X0;

		private int X1;

		private int X2;

		private int X3;

		public virtual string AlgorithmName => "Serpent";

		public virtual bool IsPartialBlockOkay => false;

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to Serpent init - " + parameters.GetType().ToString());
			}
			encrypting = forEncryption;
			wKey = MakeWorkingKey(((KeyParameter)parameters).GetKey());
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (wKey == null)
			{
				throw new InvalidOperationException("Serpent not initialised");
			}
			Check.DataLength(input, inOff, 16, "input buffer too short");
			Check.OutputLength(output, outOff, 16, "output buffer too short");
			if (encrypting)
			{
				EncryptBlock(input, inOff, output, outOff);
			}
			else
			{
				DecryptBlock(input, inOff, output, outOff);
			}
			return 16;
		}

		public virtual void Reset()
		{
		}

		private int[] MakeWorkingKey(byte[] key)
		{
			int[] array = new int[16];
			int num = 0;
			int num2 = 0;
			for (num = key.Length - 4; num > 0; num -= 4)
			{
				array[num2++] = BytesToWord(key, num);
			}
			if (num != 0)
			{
				throw new ArgumentException("key must be a multiple of 4 bytes");
			}
			array[num2++] = BytesToWord(key, 0);
			if (num2 < 8)
			{
				array[num2] = 1;
			}
			int num5 = (ROUNDS + 1) * 4;
			int[] array2 = new int[num5];
			for (int i = 8; i < 16; i++)
			{
				array[i] = RotateLeft(array[i - 8] ^ array[i - 5] ^ array[i - 3] ^ array[i - 1] ^ PHI ^ (i - 8), 11);
			}
			Array.Copy(array, 8, array2, 0, 8);
			for (int j = 8; j < num5; j++)
			{
				array2[j] = RotateLeft(array2[j - 8] ^ array2[j - 5] ^ array2[j - 3] ^ array2[j - 1] ^ PHI ^ j, 11);
			}
			Sb3(array2[0], array2[1], array2[2], array2[3]);
			array2[0] = X0;
			array2[1] = X1;
			array2[2] = X2;
			array2[3] = X3;
			Sb2(array2[4], array2[5], array2[6], array2[7]);
			array2[4] = X0;
			array2[5] = X1;
			array2[6] = X2;
			array2[7] = X3;
			Sb1(array2[8], array2[9], array2[10], array2[11]);
			array2[8] = X0;
			array2[9] = X1;
			array2[10] = X2;
			array2[11] = X3;
			Sb0(array2[12], array2[13], array2[14], array2[15]);
			array2[12] = X0;
			array2[13] = X1;
			array2[14] = X2;
			array2[15] = X3;
			Sb7(array2[16], array2[17], array2[18], array2[19]);
			array2[16] = X0;
			array2[17] = X1;
			array2[18] = X2;
			array2[19] = X3;
			Sb6(array2[20], array2[21], array2[22], array2[23]);
			array2[20] = X0;
			array2[21] = X1;
			array2[22] = X2;
			array2[23] = X3;
			Sb5(array2[24], array2[25], array2[26], array2[27]);
			array2[24] = X0;
			array2[25] = X1;
			array2[26] = X2;
			array2[27] = X3;
			Sb4(array2[28], array2[29], array2[30], array2[31]);
			array2[28] = X0;
			array2[29] = X1;
			array2[30] = X2;
			array2[31] = X3;
			Sb3(array2[32], array2[33], array2[34], array2[35]);
			array2[32] = X0;
			array2[33] = X1;
			array2[34] = X2;
			array2[35] = X3;
			Sb2(array2[36], array2[37], array2[38], array2[39]);
			array2[36] = X0;
			array2[37] = X1;
			array2[38] = X2;
			array2[39] = X3;
			Sb1(array2[40], array2[41], array2[42], array2[43]);
			array2[40] = X0;
			array2[41] = X1;
			array2[42] = X2;
			array2[43] = X3;
			Sb0(array2[44], array2[45], array2[46], array2[47]);
			array2[44] = X0;
			array2[45] = X1;
			array2[46] = X2;
			array2[47] = X3;
			Sb7(array2[48], array2[49], array2[50], array2[51]);
			array2[48] = X0;
			array2[49] = X1;
			array2[50] = X2;
			array2[51] = X3;
			Sb6(array2[52], array2[53], array2[54], array2[55]);
			array2[52] = X0;
			array2[53] = X1;
			array2[54] = X2;
			array2[55] = X3;
			Sb5(array2[56], array2[57], array2[58], array2[59]);
			array2[56] = X0;
			array2[57] = X1;
			array2[58] = X2;
			array2[59] = X3;
			Sb4(array2[60], array2[61], array2[62], array2[63]);
			array2[60] = X0;
			array2[61] = X1;
			array2[62] = X2;
			array2[63] = X3;
			Sb3(array2[64], array2[65], array2[66], array2[67]);
			array2[64] = X0;
			array2[65] = X1;
			array2[66] = X2;
			array2[67] = X3;
			Sb2(array2[68], array2[69], array2[70], array2[71]);
			array2[68] = X0;
			array2[69] = X1;
			array2[70] = X2;
			array2[71] = X3;
			Sb1(array2[72], array2[73], array2[74], array2[75]);
			array2[72] = X0;
			array2[73] = X1;
			array2[74] = X2;
			array2[75] = X3;
			Sb0(array2[76], array2[77], array2[78], array2[79]);
			array2[76] = X0;
			array2[77] = X1;
			array2[78] = X2;
			array2[79] = X3;
			Sb7(array2[80], array2[81], array2[82], array2[83]);
			array2[80] = X0;
			array2[81] = X1;
			array2[82] = X2;
			array2[83] = X3;
			Sb6(array2[84], array2[85], array2[86], array2[87]);
			array2[84] = X0;
			array2[85] = X1;
			array2[86] = X2;
			array2[87] = X3;
			Sb5(array2[88], array2[89], array2[90], array2[91]);
			array2[88] = X0;
			array2[89] = X1;
			array2[90] = X2;
			array2[91] = X3;
			Sb4(array2[92], array2[93], array2[94], array2[95]);
			array2[92] = X0;
			array2[93] = X1;
			array2[94] = X2;
			array2[95] = X3;
			Sb3(array2[96], array2[97], array2[98], array2[99]);
			array2[96] = X0;
			array2[97] = X1;
			array2[98] = X2;
			array2[99] = X3;
			Sb2(array2[100], array2[101], array2[102], array2[103]);
			array2[100] = X0;
			array2[101] = X1;
			array2[102] = X2;
			array2[103] = X3;
			Sb1(array2[104], array2[105], array2[106], array2[107]);
			array2[104] = X0;
			array2[105] = X1;
			array2[106] = X2;
			array2[107] = X3;
			Sb0(array2[108], array2[109], array2[110], array2[111]);
			array2[108] = X0;
			array2[109] = X1;
			array2[110] = X2;
			array2[111] = X3;
			Sb7(array2[112], array2[113], array2[114], array2[115]);
			array2[112] = X0;
			array2[113] = X1;
			array2[114] = X2;
			array2[115] = X3;
			Sb6(array2[116], array2[117], array2[118], array2[119]);
			array2[116] = X0;
			array2[117] = X1;
			array2[118] = X2;
			array2[119] = X3;
			Sb5(array2[120], array2[121], array2[122], array2[123]);
			array2[120] = X0;
			array2[121] = X1;
			array2[122] = X2;
			array2[123] = X3;
			Sb4(array2[124], array2[125], array2[126], array2[127]);
			array2[124] = X0;
			array2[125] = X1;
			array2[126] = X2;
			array2[127] = X3;
			Sb3(array2[128], array2[129], array2[130], array2[131]);
			array2[128] = X0;
			array2[129] = X1;
			array2[130] = X2;
			array2[131] = X3;
			return array2;
		}

		private int RotateLeft(int x, int bits)
		{
			return (x << bits) | (int)((uint)x >> 32 - bits);
		}

		private int RotateRight(int x, int bits)
		{
			return (int)((uint)x >> bits) | (x << 32 - bits);
		}

		private int BytesToWord(byte[] src, int srcOff)
		{
			return ((src[srcOff] & 0xFF) << 24) | ((src[srcOff + 1] & 0xFF) << 16) | ((src[srcOff + 2] & 0xFF) << 8) | (src[srcOff + 3] & 0xFF);
		}

		private void WordToBytes(int word, byte[] dst, int dstOff)
		{
			dst[dstOff + 3] = (byte)word;
			dst[dstOff + 2] = (byte)((uint)word >> 8);
			dst[dstOff + 1] = (byte)((uint)word >> 16);
			dst[dstOff] = (byte)((uint)word >> 24);
		}

		private void EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			X3 = BytesToWord(input, inOff);
			X2 = BytesToWord(input, inOff + 4);
			X1 = BytesToWord(input, inOff + 8);
			X0 = BytesToWord(input, inOff + 12);
			Sb0(wKey[0] ^ X0, wKey[1] ^ X1, wKey[2] ^ X2, wKey[3] ^ X3);
			LT();
			Sb1(wKey[4] ^ X0, wKey[5] ^ X1, wKey[6] ^ X2, wKey[7] ^ X3);
			LT();
			Sb2(wKey[8] ^ X0, wKey[9] ^ X1, wKey[10] ^ X2, wKey[11] ^ X3);
			LT();
			Sb3(wKey[12] ^ X0, wKey[13] ^ X1, wKey[14] ^ X2, wKey[15] ^ X3);
			LT();
			Sb4(wKey[16] ^ X0, wKey[17] ^ X1, wKey[18] ^ X2, wKey[19] ^ X3);
			LT();
			Sb5(wKey[20] ^ X0, wKey[21] ^ X1, wKey[22] ^ X2, wKey[23] ^ X3);
			LT();
			Sb6(wKey[24] ^ X0, wKey[25] ^ X1, wKey[26] ^ X2, wKey[27] ^ X3);
			LT();
			Sb7(wKey[28] ^ X0, wKey[29] ^ X1, wKey[30] ^ X2, wKey[31] ^ X3);
			LT();
			Sb0(wKey[32] ^ X0, wKey[33] ^ X1, wKey[34] ^ X2, wKey[35] ^ X3);
			LT();
			Sb1(wKey[36] ^ X0, wKey[37] ^ X1, wKey[38] ^ X2, wKey[39] ^ X3);
			LT();
			Sb2(wKey[40] ^ X0, wKey[41] ^ X1, wKey[42] ^ X2, wKey[43] ^ X3);
			LT();
			Sb3(wKey[44] ^ X0, wKey[45] ^ X1, wKey[46] ^ X2, wKey[47] ^ X3);
			LT();
			Sb4(wKey[48] ^ X0, wKey[49] ^ X1, wKey[50] ^ X2, wKey[51] ^ X3);
			LT();
			Sb5(wKey[52] ^ X0, wKey[53] ^ X1, wKey[54] ^ X2, wKey[55] ^ X3);
			LT();
			Sb6(wKey[56] ^ X0, wKey[57] ^ X1, wKey[58] ^ X2, wKey[59] ^ X3);
			LT();
			Sb7(wKey[60] ^ X0, wKey[61] ^ X1, wKey[62] ^ X2, wKey[63] ^ X3);
			LT();
			Sb0(wKey[64] ^ X0, wKey[65] ^ X1, wKey[66] ^ X2, wKey[67] ^ X3);
			LT();
			Sb1(wKey[68] ^ X0, wKey[69] ^ X1, wKey[70] ^ X2, wKey[71] ^ X3);
			LT();
			Sb2(wKey[72] ^ X0, wKey[73] ^ X1, wKey[74] ^ X2, wKey[75] ^ X3);
			LT();
			Sb3(wKey[76] ^ X0, wKey[77] ^ X1, wKey[78] ^ X2, wKey[79] ^ X3);
			LT();
			Sb4(wKey[80] ^ X0, wKey[81] ^ X1, wKey[82] ^ X2, wKey[83] ^ X3);
			LT();
			Sb5(wKey[84] ^ X0, wKey[85] ^ X1, wKey[86] ^ X2, wKey[87] ^ X3);
			LT();
			Sb6(wKey[88] ^ X0, wKey[89] ^ X1, wKey[90] ^ X2, wKey[91] ^ X3);
			LT();
			Sb7(wKey[92] ^ X0, wKey[93] ^ X1, wKey[94] ^ X2, wKey[95] ^ X3);
			LT();
			Sb0(wKey[96] ^ X0, wKey[97] ^ X1, wKey[98] ^ X2, wKey[99] ^ X3);
			LT();
			Sb1(wKey[100] ^ X0, wKey[101] ^ X1, wKey[102] ^ X2, wKey[103] ^ X3);
			LT();
			Sb2(wKey[104] ^ X0, wKey[105] ^ X1, wKey[106] ^ X2, wKey[107] ^ X3);
			LT();
			Sb3(wKey[108] ^ X0, wKey[109] ^ X1, wKey[110] ^ X2, wKey[111] ^ X3);
			LT();
			Sb4(wKey[112] ^ X0, wKey[113] ^ X1, wKey[114] ^ X2, wKey[115] ^ X3);
			LT();
			Sb5(wKey[116] ^ X0, wKey[117] ^ X1, wKey[118] ^ X2, wKey[119] ^ X3);
			LT();
			Sb6(wKey[120] ^ X0, wKey[121] ^ X1, wKey[122] ^ X2, wKey[123] ^ X3);
			LT();
			Sb7(wKey[124] ^ X0, wKey[125] ^ X1, wKey[126] ^ X2, wKey[127] ^ X3);
			WordToBytes(wKey[131] ^ X3, outBytes, outOff);
			WordToBytes(wKey[130] ^ X2, outBytes, outOff + 4);
			WordToBytes(wKey[129] ^ X1, outBytes, outOff + 8);
			WordToBytes(wKey[128] ^ X0, outBytes, outOff + 12);
		}

		private void DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
		{
			X3 = (wKey[131] ^ BytesToWord(input, inOff));
			X2 = (wKey[130] ^ BytesToWord(input, inOff + 4));
			X1 = (wKey[129] ^ BytesToWord(input, inOff + 8));
			X0 = (wKey[128] ^ BytesToWord(input, inOff + 12));
			Ib7(X0, X1, X2, X3);
			X0 ^= wKey[124];
			X1 ^= wKey[125];
			X2 ^= wKey[126];
			X3 ^= wKey[127];
			InverseLT();
			Ib6(X0, X1, X2, X3);
			X0 ^= wKey[120];
			X1 ^= wKey[121];
			X2 ^= wKey[122];
			X3 ^= wKey[123];
			InverseLT();
			Ib5(X0, X1, X2, X3);
			X0 ^= wKey[116];
			X1 ^= wKey[117];
			X2 ^= wKey[118];
			X3 ^= wKey[119];
			InverseLT();
			Ib4(X0, X1, X2, X3);
			X0 ^= wKey[112];
			X1 ^= wKey[113];
			X2 ^= wKey[114];
			X3 ^= wKey[115];
			InverseLT();
			Ib3(X0, X1, X2, X3);
			X0 ^= wKey[108];
			X1 ^= wKey[109];
			X2 ^= wKey[110];
			X3 ^= wKey[111];
			InverseLT();
			Ib2(X0, X1, X2, X3);
			X0 ^= wKey[104];
			X1 ^= wKey[105];
			X2 ^= wKey[106];
			X3 ^= wKey[107];
			InverseLT();
			Ib1(X0, X1, X2, X3);
			X0 ^= wKey[100];
			X1 ^= wKey[101];
			X2 ^= wKey[102];
			X3 ^= wKey[103];
			InverseLT();
			Ib0(X0, X1, X2, X3);
			X0 ^= wKey[96];
			X1 ^= wKey[97];
			X2 ^= wKey[98];
			X3 ^= wKey[99];
			InverseLT();
			Ib7(X0, X1, X2, X3);
			X0 ^= wKey[92];
			X1 ^= wKey[93];
			X2 ^= wKey[94];
			X3 ^= wKey[95];
			InverseLT();
			Ib6(X0, X1, X2, X3);
			X0 ^= wKey[88];
			X1 ^= wKey[89];
			X2 ^= wKey[90];
			X3 ^= wKey[91];
			InverseLT();
			Ib5(X0, X1, X2, X3);
			X0 ^= wKey[84];
			X1 ^= wKey[85];
			X2 ^= wKey[86];
			X3 ^= wKey[87];
			InverseLT();
			Ib4(X0, X1, X2, X3);
			X0 ^= wKey[80];
			X1 ^= wKey[81];
			X2 ^= wKey[82];
			X3 ^= wKey[83];
			InverseLT();
			Ib3(X0, X1, X2, X3);
			X0 ^= wKey[76];
			X1 ^= wKey[77];
			X2 ^= wKey[78];
			X3 ^= wKey[79];
			InverseLT();
			Ib2(X0, X1, X2, X3);
			X0 ^= wKey[72];
			X1 ^= wKey[73];
			X2 ^= wKey[74];
			X3 ^= wKey[75];
			InverseLT();
			Ib1(X0, X1, X2, X3);
			X0 ^= wKey[68];
			X1 ^= wKey[69];
			X2 ^= wKey[70];
			X3 ^= wKey[71];
			InverseLT();
			Ib0(X0, X1, X2, X3);
			X0 ^= wKey[64];
			X1 ^= wKey[65];
			X2 ^= wKey[66];
			X3 ^= wKey[67];
			InverseLT();
			Ib7(X0, X1, X2, X3);
			X0 ^= wKey[60];
			X1 ^= wKey[61];
			X2 ^= wKey[62];
			X3 ^= wKey[63];
			InverseLT();
			Ib6(X0, X1, X2, X3);
			X0 ^= wKey[56];
			X1 ^= wKey[57];
			X2 ^= wKey[58];
			X3 ^= wKey[59];
			InverseLT();
			Ib5(X0, X1, X2, X3);
			X0 ^= wKey[52];
			X1 ^= wKey[53];
			X2 ^= wKey[54];
			X3 ^= wKey[55];
			InverseLT();
			Ib4(X0, X1, X2, X3);
			X0 ^= wKey[48];
			X1 ^= wKey[49];
			X2 ^= wKey[50];
			X3 ^= wKey[51];
			InverseLT();
			Ib3(X0, X1, X2, X3);
			X0 ^= wKey[44];
			X1 ^= wKey[45];
			X2 ^= wKey[46];
			X3 ^= wKey[47];
			InverseLT();
			Ib2(X0, X1, X2, X3);
			X0 ^= wKey[40];
			X1 ^= wKey[41];
			X2 ^= wKey[42];
			X3 ^= wKey[43];
			InverseLT();
			Ib1(X0, X1, X2, X3);
			X0 ^= wKey[36];
			X1 ^= wKey[37];
			X2 ^= wKey[38];
			X3 ^= wKey[39];
			InverseLT();
			Ib0(X0, X1, X2, X3);
			X0 ^= wKey[32];
			X1 ^= wKey[33];
			X2 ^= wKey[34];
			X3 ^= wKey[35];
			InverseLT();
			Ib7(X0, X1, X2, X3);
			X0 ^= wKey[28];
			X1 ^= wKey[29];
			X2 ^= wKey[30];
			X3 ^= wKey[31];
			InverseLT();
			Ib6(X0, X1, X2, X3);
			X0 ^= wKey[24];
			X1 ^= wKey[25];
			X2 ^= wKey[26];
			X3 ^= wKey[27];
			InverseLT();
			Ib5(X0, X1, X2, X3);
			X0 ^= wKey[20];
			X1 ^= wKey[21];
			X2 ^= wKey[22];
			X3 ^= wKey[23];
			InverseLT();
			Ib4(X0, X1, X2, X3);
			X0 ^= wKey[16];
			X1 ^= wKey[17];
			X2 ^= wKey[18];
			X3 ^= wKey[19];
			InverseLT();
			Ib3(X0, X1, X2, X3);
			X0 ^= wKey[12];
			X1 ^= wKey[13];
			X2 ^= wKey[14];
			X3 ^= wKey[15];
			InverseLT();
			Ib2(X0, X1, X2, X3);
			X0 ^= wKey[8];
			X1 ^= wKey[9];
			X2 ^= wKey[10];
			X3 ^= wKey[11];
			InverseLT();
			Ib1(X0, X1, X2, X3);
			X0 ^= wKey[4];
			X1 ^= wKey[5];
			X2 ^= wKey[6];
			X3 ^= wKey[7];
			InverseLT();
			Ib0(X0, X1, X2, X3);
			WordToBytes(X3 ^ wKey[3], outBytes, outOff);
			WordToBytes(X2 ^ wKey[2], outBytes, outOff + 4);
			WordToBytes(X1 ^ wKey[1], outBytes, outOff + 8);
			WordToBytes(X0 ^ wKey[0], outBytes, outOff + 12);
		}

		private void Sb0(int a, int b, int c, int d)
		{
			int num = a ^ d;
			int num2 = c ^ num;
			int num3 = b ^ num2;
			X3 = ((a & d) ^ num3);
			int num4 = a ^ (b & num);
			X2 = (num3 ^ (c | num4));
			int num5 = X3 & (num2 ^ num4);
			X1 = (~num2 ^ num5);
			X0 = (num5 ^ ~num4);
		}

		private void Ib0(int a, int b, int c, int d)
		{
			int num = ~a;
			int num2 = a ^ b;
			int num3 = d ^ (num | num2);
			int num4 = c ^ num3;
			X2 = (num2 ^ num4);
			int num5 = num ^ (d & num2);
			X1 = (num3 ^ (X2 & num5));
			X3 = ((a & num3) ^ (num4 | X1));
			X0 = (X3 ^ (num4 ^ num5));
		}

		private void Sb1(int a, int b, int c, int d)
		{
			int num = b ^ ~a;
			int num2 = c ^ (a | num);
			X2 = (d ^ num2);
			int num3 = b ^ (d | num);
			int num4 = num ^ X2;
			X3 = (num4 ^ (num2 & num3));
			int num5 = num2 ^ num3;
			X1 = (X3 ^ num5);
			X0 = (num2 ^ (num4 & num5));
		}

		private void Ib1(int a, int b, int c, int d)
		{
			int num = b ^ d;
			int num2 = a ^ (b & num);
			int num3 = num ^ num2;
			X3 = (c ^ num3);
			int num4 = b ^ (num & num2);
			int num5 = X3 | num4;
			X1 = (num2 ^ num5);
			int num6 = ~X1;
			int num7 = X3 ^ num4;
			X0 = (num6 ^ num7);
			X2 = (num3 ^ (num6 | num7));
		}

		private void Sb2(int a, int b, int c, int d)
		{
			int num = ~a;
			int num2 = b ^ d;
			int num3 = c & num;
			X0 = (num2 ^ num3);
			int num4 = c ^ num;
			int num5 = c ^ X0;
			int num6 = b & num5;
			X3 = (num4 ^ num6);
			X2 = (a ^ ((d | num6) & (X0 | num4)));
			X1 = (num2 ^ X3 ^ (X2 ^ (d | num)));
		}

		private void Ib2(int a, int b, int c, int d)
		{
			int num = b ^ d;
			int num2 = ~num;
			int num3 = a ^ c;
			int num4 = c ^ num;
			int num5 = b & num4;
			X0 = (num3 ^ num5);
			int num6 = a | num2;
			int num7 = d ^ num6;
			int num8 = num3 | num7;
			X3 = (num ^ num8);
			int num9 = ~num4;
			int num10 = X0 | X3;
			X1 = (num9 ^ num10);
			X2 = ((d & num9) ^ (num3 ^ num10));
		}

		private void Sb3(int a, int b, int c, int d)
		{
			int num = a ^ b;
			int num2 = a & c;
			int num3 = a | d;
			int num4 = c ^ d;
			int num5 = num & num3;
			int num6 = num2 | num5;
			X2 = (num4 ^ num6);
			int num7 = b ^ num3;
			int num8 = num6 ^ num7;
			int num9 = num4 & num8;
			X0 = (num ^ num9);
			int num10 = X2 & X0;
			X1 = (num8 ^ num10);
			X3 = ((b | d) ^ (num4 ^ num10));
		}

		private void Ib3(int a, int b, int c, int d)
		{
			int num = a | b;
			int num2 = b ^ c;
			int num3 = b & num2;
			int num4 = a ^ num3;
			int num5 = c ^ num4;
			int num6 = d | num4;
			X0 = (num2 ^ num6);
			int num7 = num2 | num6;
			int num8 = d ^ num7;
			X2 = (num5 ^ num8);
			int num9 = num ^ num8;
			int num10 = X0 & num9;
			X3 = (num4 ^ num10);
			X1 = (X3 ^ (X0 ^ num9));
		}

		private void Sb4(int a, int b, int c, int d)
		{
			int num = a ^ d;
			int num2 = d & num;
			int num3 = c ^ num2;
			int num4 = b | num3;
			X3 = (num ^ num4);
			int num5 = ~b;
			int num6 = num | num5;
			X0 = (num3 ^ num6);
			int num7 = a & X0;
			int num8 = num ^ num5;
			int num9 = num4 & num8;
			X2 = (num7 ^ num9);
			X1 = (a ^ num3 ^ (num8 & X2));
		}

		private void Ib4(int a, int b, int c, int d)
		{
			int num = c | d;
			int num2 = a & num;
			int num3 = b ^ num2;
			int num4 = a & num3;
			int num5 = c ^ num4;
			X1 = (d ^ num5);
			int num6 = ~a;
			int num7 = num5 & X1;
			X3 = (num3 ^ num7);
			int num8 = X1 | num6;
			int num9 = d ^ num8;
			X0 = (X3 ^ num9);
			X2 = ((num3 & num9) ^ (X1 ^ num6));
		}

		private void Sb5(int a, int b, int c, int d)
		{
			int num = ~a;
			int num2 = a ^ b;
			int num3 = a ^ d;
			int num4 = c ^ num;
			int num5 = num2 | num3;
			X0 = (num4 ^ num5);
			int num6 = d & X0;
			int num7 = num2 ^ X0;
			X1 = (num6 ^ num7);
			int num8 = num | X0;
			int num9 = num2 | num6;
			int num10 = num3 ^ num8;
			X2 = (num9 ^ num10);
			X3 = (b ^ num6 ^ (X1 & num10));
		}

		private void Ib5(int a, int b, int c, int d)
		{
			int num = ~c;
			int num2 = b & num;
			int num3 = d ^ num2;
			int num4 = a & num3;
			int num5 = b ^ num;
			X3 = (num4 ^ num5);
			int num6 = b | X3;
			int num7 = a & num6;
			X1 = (num3 ^ num7);
			int num8 = a | d;
			int num9 = num ^ num6;
			X0 = (num8 ^ num9);
			X2 = ((b & num8) ^ (num4 | (a ^ c)));
		}

		private void Sb6(int a, int b, int c, int d)
		{
			int num = ~a;
			int num2 = a ^ d;
			int num3 = b ^ num2;
			int num4 = num | num2;
			int num5 = c ^ num4;
			X1 = (b ^ num5);
			int num6 = num2 | X1;
			int num7 = d ^ num6;
			int num8 = num5 & num7;
			X2 = (num3 ^ num8);
			int num9 = num5 ^ num7;
			X0 = (X2 ^ num9);
			X3 = (~num5 ^ (num3 & num9));
		}

		private void Ib6(int a, int b, int c, int d)
		{
			int num = ~a;
			int num2 = a ^ b;
			int num3 = c ^ num2;
			int num4 = c | num;
			int num5 = d ^ num4;
			X1 = (num3 ^ num5);
			int num6 = num3 & num5;
			int num7 = num2 ^ num6;
			int num8 = b | num7;
			X3 = (num5 ^ num8);
			int num9 = b | X3;
			X0 = (num7 ^ num9);
			X2 = ((d & num) ^ (num3 ^ num9));
		}

		private void Sb7(int a, int b, int c, int d)
		{
			int num = b ^ c;
			int num2 = c & num;
			int num3 = d ^ num2;
			int num4 = a ^ num3;
			int num5 = d | num;
			int num6 = num4 & num5;
			X1 = (b ^ num6);
			int num7 = num3 | X1;
			int num8 = a & num4;
			X3 = (num ^ num8);
			int num9 = num4 ^ num7;
			int num10 = X3 & num9;
			X2 = (num3 ^ num10);
			X0 = (~num9 ^ (X3 & X2));
		}

		private void Ib7(int a, int b, int c, int d)
		{
			int num = c | (a & b);
			int num2 = d & (a | b);
			X3 = (num ^ num2);
			int num3 = ~d;
			int num4 = b ^ num2;
			int num5 = num4 | (X3 ^ num3);
			X1 = (a ^ num5);
			X0 = (c ^ num4 ^ (d | X1));
			X2 = (num ^ X1 ^ (X0 ^ (a & X3)));
		}

		private void LT()
		{
			int num = RotateLeft(X0, 13);
			int num2 = RotateLeft(X2, 3);
			int x = X1 ^ num ^ num2;
			int x2 = X3 ^ num2 ^ (num << 3);
			X1 = RotateLeft(x, 1);
			X3 = RotateLeft(x2, 7);
			X0 = RotateLeft(num ^ X1 ^ X3, 5);
			X2 = RotateLeft(num2 ^ X3 ^ (X1 << 7), 22);
		}

		private void InverseLT()
		{
			int num = RotateRight(X2, 22) ^ X3 ^ (X1 << 7);
			int num2 = RotateRight(X0, 5) ^ X1 ^ X3;
			int num3 = RotateRight(X3, 7);
			int num4 = RotateRight(X1, 1);
			X3 = (num3 ^ num ^ (num2 << 3));
			X1 = (num4 ^ num2 ^ num);
			X2 = RotateRight(num, 3);
			X0 = RotateRight(num2, 13);
		}
	}
}
