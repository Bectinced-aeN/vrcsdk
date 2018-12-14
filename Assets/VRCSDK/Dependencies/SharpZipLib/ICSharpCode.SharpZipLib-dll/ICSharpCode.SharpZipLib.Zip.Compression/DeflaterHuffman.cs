using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	public class DeflaterHuffman
	{
		private class Tree
		{
			public short[] freqs;

			public byte[] length;

			public int minNumCodes;

			public int numCodes;

			private short[] codes;

			private readonly int[] bl_counts;

			private readonly int maxLength;

			private DeflaterHuffman dh;

			public Tree(DeflaterHuffman dh, int elems, int minCodes, int maxLength)
			{
				this.dh = dh;
				minNumCodes = minCodes;
				this.maxLength = maxLength;
				freqs = new short[elems];
				bl_counts = new int[maxLength];
			}

			public void Reset()
			{
				for (int i = 0; i < freqs.Length; i++)
				{
					freqs[i] = 0;
				}
				codes = null;
				length = null;
			}

			public void WriteSymbol(int code)
			{
				dh.pending.WriteBits(codes[code] & 0xFFFF, length[code]);
			}

			public void CheckEmpty()
			{
				bool flag = true;
				for (int i = 0; i < freqs.Length; i++)
				{
					flag &= (freqs[i] == 0);
				}
				if (!flag)
				{
					throw new SharpZipBaseException("!Empty");
				}
			}

			public void SetStaticCodes(short[] staticCodes, byte[] staticLengths)
			{
				codes = staticCodes;
				length = staticLengths;
			}

			public void BuildCodes()
			{
				int num3 = freqs.Length;
				int[] array = new int[maxLength];
				int num = 0;
				codes = new short[freqs.Length];
				for (int i = 0; i < maxLength; i++)
				{
					array[i] = num;
					num += bl_counts[i] << 15 - i;
				}
				for (int j = 0; j < numCodes; j++)
				{
					int num2 = length[j];
					if (num2 > 0)
					{
						codes[j] = BitReverse(array[num2 - 1]);
						array[num2 - 1] += 1 << 16 - num2;
					}
				}
			}

			public void BuildTree()
			{
				int num = freqs.Length;
				int[] array = new int[num];
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < num; i++)
				{
					int num4 = freqs[i];
					if (num4 != 0)
					{
						int num6 = num2++;
						int num7;
						while (num6 > 0 && freqs[array[num7 = (num6 - 1) / 2]] > num4)
						{
							array[num6] = array[num7];
							num6 = num7;
						}
						array[num6] = i;
						num3 = i;
					}
				}
				while (num2 < 2)
				{
					int num8 = (num3 < 2) ? (++num3) : 0;
					array[num2++] = num8;
				}
				numCodes = Math.Max(num3 + 1, minNumCodes);
				int num10 = num2;
				int[] array2 = new int[4 * num2 - 2];
				int[] array3 = new int[2 * num2 - 1];
				int num11 = num10;
				for (int j = 0; j < num2; j++)
				{
					int num12 = array2[2 * j] = array[j];
					array2[2 * j + 1] = -1;
					array3[j] = freqs[num12] << 8;
					array[j] = j;
				}
				do
				{
					int num13 = array[0];
					int num14 = array[--num2];
					int num15 = 0;
					int num16;
					for (num16 = 1; num16 < num2; num16 = num16 * 2 + 1)
					{
						if (num16 + 1 < num2 && array3[array[num16]] > array3[array[num16 + 1]])
						{
							num16++;
						}
						array[num15] = array[num16];
						num15 = num16;
					}
					int num17 = array3[num14];
					while ((num16 = num15) > 0 && array3[array[num15 = (num16 - 1) / 2]] > num17)
					{
						array[num16] = array[num15];
					}
					array[num16] = num14;
					int num18 = array[0];
					num14 = num11++;
					array2[2 * num14] = num13;
					array2[2 * num14 + 1] = num18;
					int num20 = Math.Min(array3[num13] & 0xFF, array3[num18] & 0xFF);
					num17 = (array3[num14] = array3[num13] + array3[num18] - num20 + 1);
					num15 = 0;
					for (num16 = 1; num16 < num2; num16 = num15 * 2 + 1)
					{
						if (num16 + 1 < num2 && array3[array[num16]] > array3[array[num16 + 1]])
						{
							num16++;
						}
						array[num15] = array[num16];
						num15 = num16;
					}
					while ((num16 = num15) > 0 && array3[array[num15 = (num16 - 1) / 2]] > num17)
					{
						array[num16] = array[num15];
					}
					array[num16] = num14;
				}
				while (num2 > 1);
				if (array[0] != array2.Length / 2 - 1)
				{
					throw new SharpZipBaseException("Heap invariant violated");
				}
				BuildLength(array2);
			}

			public int GetEncodedLength()
			{
				int num = 0;
				for (int i = 0; i < freqs.Length; i++)
				{
					num += freqs[i] * length[i];
				}
				return num;
			}

			public void CalcBLFreq(Tree blTree)
			{
				int num = -1;
				int num2 = 0;
				while (num2 < numCodes)
				{
					int num3 = 1;
					int num4 = length[num2];
					int num5;
					int num6;
					if (num4 == 0)
					{
						num5 = 138;
						num6 = 3;
					}
					else
					{
						num5 = 6;
						num6 = 3;
						if (num != num4)
						{
							blTree.freqs[num4]++;
							num3 = 0;
						}
					}
					num = num4;
					num2++;
					while (num2 < numCodes && num == length[num2])
					{
						num2++;
						if (++num3 >= num5)
						{
							break;
						}
					}
					if (num3 < num6)
					{
						blTree.freqs[num] += (short)num3;
					}
					else if (num != 0)
					{
						blTree.freqs[16]++;
					}
					else if (num3 <= 10)
					{
						blTree.freqs[17]++;
					}
					else
					{
						blTree.freqs[18]++;
					}
				}
			}

			public void WriteTree(Tree blTree)
			{
				int num = -1;
				int num2 = 0;
				while (num2 < numCodes)
				{
					int num3 = 1;
					int num4 = length[num2];
					int num5;
					int num6;
					if (num4 == 0)
					{
						num5 = 138;
						num6 = 3;
					}
					else
					{
						num5 = 6;
						num6 = 3;
						if (num != num4)
						{
							blTree.WriteSymbol(num4);
							num3 = 0;
						}
					}
					num = num4;
					num2++;
					while (num2 < numCodes && num == length[num2])
					{
						num2++;
						if (++num3 >= num5)
						{
							break;
						}
					}
					if (num3 < num6)
					{
						while (num3-- > 0)
						{
							blTree.WriteSymbol(num);
						}
					}
					else if (num != 0)
					{
						blTree.WriteSymbol(16);
						dh.pending.WriteBits(num3 - 3, 2);
					}
					else if (num3 <= 10)
					{
						blTree.WriteSymbol(17);
						dh.pending.WriteBits(num3 - 3, 3);
					}
					else
					{
						blTree.WriteSymbol(18);
						dh.pending.WriteBits(num3 - 11, 7);
					}
				}
			}

			private void BuildLength(int[] childs)
			{
				length = new byte[freqs.Length];
				int num = childs.Length / 2;
				int num2 = (num + 1) / 2;
				int num3 = 0;
				for (int i = 0; i < maxLength; i++)
				{
					bl_counts[i] = 0;
				}
				int[] array = new int[num];
				array[num - 1] = 0;
				for (int num4 = num - 1; num4 >= 0; num4--)
				{
					if (childs[2 * num4 + 1] != -1)
					{
						int num5 = array[num4] + 1;
						if (num5 > maxLength)
						{
							num5 = maxLength;
							num3++;
						}
						array[childs[2 * num4]] = (array[childs[2 * num4 + 1]] = num5);
					}
					else
					{
						int num6 = array[num4];
						bl_counts[num6 - 1]++;
						length[childs[2 * num4]] = (byte)array[num4];
					}
				}
				if (num3 != 0)
				{
					int num7 = maxLength - 1;
					while (true)
					{
						if (bl_counts[--num7] != 0)
						{
							do
							{
								bl_counts[num7]--;
								bl_counts[++num7]++;
								num3 -= 1 << maxLength - 1 - num7;
							}
							while (num3 > 0 && num7 < maxLength - 1);
							if (num3 <= 0)
							{
								break;
							}
						}
					}
					bl_counts[maxLength - 1] += num3;
					bl_counts[maxLength - 2] -= num3;
					int num8 = 2 * num2;
					for (int num9 = maxLength; num9 != 0; num9--)
					{
						int num10 = bl_counts[num9 - 1];
						while (num10 > 0)
						{
							int num12 = 2 * childs[num8++];
							if (childs[num12 + 1] == -1)
							{
								length[childs[num12]] = (byte)num9;
								num10--;
							}
						}
					}
				}
			}
		}

		private const int BUFSIZE = 16384;

		private const int LITERAL_NUM = 286;

		private const int DIST_NUM = 30;

		private const int BITLEN_NUM = 19;

		private const int REP_3_6 = 16;

		private const int REP_3_10 = 17;

		private const int REP_11_138 = 18;

		private const int EOF_SYMBOL = 256;

		private static readonly int[] BL_ORDER;

		private static readonly byte[] bit4Reverse;

		private static short[] staticLCodes;

		private static byte[] staticLLength;

		private static short[] staticDCodes;

		private static byte[] staticDLength;

		public DeflaterPending pending;

		private Tree literalTree;

		private Tree distTree;

		private Tree blTree;

		private short[] d_buf;

		private byte[] l_buf;

		private int last_lit;

		private int extra_bits;

		static DeflaterHuffman()
		{
			BL_ORDER = new int[19]
			{
				16,
				17,
				18,
				0,
				8,
				7,
				9,
				6,
				10,
				5,
				11,
				4,
				12,
				3,
				13,
				2,
				14,
				1,
				15
			};
			bit4Reverse = new byte[16]
			{
				0,
				8,
				4,
				12,
				2,
				10,
				6,
				14,
				1,
				9,
				5,
				13,
				3,
				11,
				7,
				15
			};
			staticLCodes = new short[286];
			staticLLength = new byte[286];
			int num = 0;
			while (num < 144)
			{
				staticLCodes[num] = BitReverse(48 + num << 8);
				staticLLength[num++] = 8;
			}
			while (num < 256)
			{
				staticLCodes[num] = BitReverse(256 + num << 7);
				staticLLength[num++] = 9;
			}
			while (num < 280)
			{
				staticLCodes[num] = BitReverse(-256 + num << 9);
				staticLLength[num++] = 7;
			}
			while (num < 286)
			{
				staticLCodes[num] = BitReverse(-88 + num << 8);
				staticLLength[num++] = 8;
			}
			staticDCodes = new short[30];
			staticDLength = new byte[30];
			for (num = 0; num < 30; num++)
			{
				staticDCodes[num] = BitReverse(num << 11);
				staticDLength[num] = 5;
			}
		}

		public DeflaterHuffman(DeflaterPending pending)
		{
			this.pending = pending;
			literalTree = new Tree(this, 286, 257, 15);
			distTree = new Tree(this, 30, 1, 15);
			blTree = new Tree(this, 19, 4, 7);
			d_buf = new short[16384];
			l_buf = new byte[16384];
		}

		public void Reset()
		{
			last_lit = 0;
			extra_bits = 0;
			literalTree.Reset();
			distTree.Reset();
			blTree.Reset();
		}

		public void SendAllTrees(int blTreeCodes)
		{
			blTree.BuildCodes();
			literalTree.BuildCodes();
			distTree.BuildCodes();
			pending.WriteBits(literalTree.numCodes - 257, 5);
			pending.WriteBits(distTree.numCodes - 1, 5);
			pending.WriteBits(blTreeCodes - 4, 4);
			for (int i = 0; i < blTreeCodes; i++)
			{
				pending.WriteBits(blTree.length[BL_ORDER[i]], 3);
			}
			literalTree.WriteTree(blTree);
			distTree.WriteTree(blTree);
		}

		public void CompressBlock()
		{
			for (int i = 0; i < last_lit; i++)
			{
				int num = l_buf[i] & 0xFF;
				int num2 = d_buf[i];
				if (num2-- != 0)
				{
					int num4 = Lcode(num);
					literalTree.WriteSymbol(num4);
					int num5 = (num4 - 261) / 4;
					if (num5 > 0 && num5 <= 5)
					{
						pending.WriteBits(num & ((1 << num5) - 1), num5);
					}
					int num6 = Dcode(num2);
					distTree.WriteSymbol(num6);
					num5 = num6 / 2 - 1;
					if (num5 > 0)
					{
						pending.WriteBits(num2 & ((1 << num5) - 1), num5);
					}
				}
				else
				{
					literalTree.WriteSymbol(num);
				}
			}
			literalTree.WriteSymbol(256);
		}

		public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
		{
			pending.WriteBits(lastBlock ? 1 : 0, 3);
			pending.AlignToByte();
			pending.WriteShort(storedLength);
			pending.WriteShort(~storedLength);
			pending.WriteBlock(stored, storedOffset, storedLength);
			Reset();
		}

		public void FlushBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
		{
			literalTree.freqs[256]++;
			literalTree.BuildTree();
			distTree.BuildTree();
			literalTree.CalcBLFreq(blTree);
			distTree.CalcBLFreq(blTree);
			blTree.BuildTree();
			int num = 4;
			for (int num2 = 18; num2 > num; num2--)
			{
				if (blTree.length[BL_ORDER[num2]] > 0)
				{
					num = num2 + 1;
				}
			}
			int num3 = 14 + num * 3 + blTree.GetEncodedLength() + literalTree.GetEncodedLength() + distTree.GetEncodedLength() + extra_bits;
			int num4 = extra_bits;
			for (int i = 0; i < 286; i++)
			{
				num4 += literalTree.freqs[i] * staticLLength[i];
			}
			for (int j = 0; j < 30; j++)
			{
				num4 += distTree.freqs[j] * staticDLength[j];
			}
			if (num3 >= num4)
			{
				num3 = num4;
			}
			if (storedOffset >= 0 && storedLength + 4 < num3 >> 3)
			{
				FlushStoredBlock(stored, storedOffset, storedLength, lastBlock);
			}
			else if (num3 == num4)
			{
				pending.WriteBits(2 + (lastBlock ? 1 : 0), 3);
				literalTree.SetStaticCodes(staticLCodes, staticLLength);
				distTree.SetStaticCodes(staticDCodes, staticDLength);
				CompressBlock();
				Reset();
			}
			else
			{
				pending.WriteBits(4 + (lastBlock ? 1 : 0), 3);
				SendAllTrees(num);
				CompressBlock();
				Reset();
			}
		}

		public bool IsFull()
		{
			return last_lit >= 16384;
		}

		public bool TallyLit(int literal)
		{
			d_buf[last_lit] = 0;
			l_buf[last_lit++] = (byte)literal;
			literalTree.freqs[literal]++;
			return IsFull();
		}

		public bool TallyDist(int distance, int length)
		{
			d_buf[last_lit] = (short)distance;
			l_buf[last_lit++] = (byte)(length - 3);
			int num = Lcode(length - 3);
			literalTree.freqs[num]++;
			if (num >= 265 && num < 285)
			{
				extra_bits += (num - 261) / 4;
			}
			int num2 = Dcode(distance - 1);
			distTree.freqs[num2]++;
			if (num2 >= 4)
			{
				extra_bits += num2 / 2 - 1;
			}
			return IsFull();
		}

		public static short BitReverse(int toReverse)
		{
			return (short)((bit4Reverse[toReverse & 0xF] << 12) | (bit4Reverse[(toReverse >> 4) & 0xF] << 8) | (bit4Reverse[(toReverse >> 8) & 0xF] << 4) | bit4Reverse[toReverse >> 12]);
		}

		private static int Lcode(int length)
		{
			if (length == 255)
			{
				return 285;
			}
			int num = 257;
			while (length >= 8)
			{
				num += 4;
				length >>= 1;
			}
			return num + length;
		}

		private static int Dcode(int distance)
		{
			int num = 0;
			while (distance >= 4)
			{
				num += 2;
				distance >>= 1;
			}
			return num + distance;
		}
	}
}
