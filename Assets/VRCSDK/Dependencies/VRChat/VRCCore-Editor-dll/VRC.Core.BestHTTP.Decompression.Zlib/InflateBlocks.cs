using System;

namespace VRC.Core.BestHTTP.Decompression.Zlib
{
	internal sealed class InflateBlocks
	{
		private enum InflateBlockMode
		{
			TYPE,
			LENS,
			STORED,
			TABLE,
			BTREE,
			DTREE,
			CODES,
			DRY,
			DONE,
			BAD
		}

		private const int MANY = 1440;

		internal static readonly int[] border = new int[19]
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

		private InflateBlockMode mode;

		internal int left;

		internal int table;

		internal int index;

		internal int[] blens;

		internal int[] bb = new int[1];

		internal int[] tb = new int[1];

		internal InflateCodes codes = new InflateCodes();

		internal int last;

		internal ZlibCodec _codec;

		internal int bitk;

		internal int bitb;

		internal int[] hufts;

		internal byte[] window;

		internal int end;

		internal int readAt;

		internal int writeAt;

		internal object checkfn;

		internal uint check;

		internal InfTree inftree = new InfTree();

		internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
		{
			_codec = codec;
			hufts = new int[4320];
			window = new byte[w];
			end = w;
			this.checkfn = checkfn;
			mode = InflateBlockMode.TYPE;
			Reset();
		}

		internal uint Reset()
		{
			uint result = check;
			mode = InflateBlockMode.TYPE;
			bitk = 0;
			bitb = 0;
			readAt = (writeAt = 0);
			if (checkfn != null)
			{
				_codec._Adler32 = (check = Adler.Adler32(0u, null, 0, 0));
			}
			return result;
		}

		internal int Process(int r)
		{
			int num = _codec.NextIn;
			int num2 = _codec.AvailableBytesIn;
			int num3 = bitb;
			int i = bitk;
			int num4 = writeAt;
			int num5 = (num4 >= readAt) ? (end - num4) : (readAt - num4 - 1);
			while (true)
			{
				switch (mode)
				{
				case InflateBlockMode.TYPE:
				{
					for (; i < 3; i += 8)
					{
						if (num2 == 0)
						{
							bitb = num3;
							bitk = i;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num4;
							return Flush(r);
						}
						r = 0;
						num2--;
						num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
					}
					int num7 = num3 & 7;
					last = (num7 & 1);
					switch ((uint)num7 >> 1)
					{
					case 0u:
						num3 >>= 3;
						i -= 3;
						num7 = (i & 7);
						num3 >>= num7;
						i -= num7;
						mode = InflateBlockMode.LENS;
						break;
					case 1u:
					{
						int[] array = new int[1];
						int[] array2 = new int[1];
						int[][] array3 = new int[1][];
						int[][] array4 = new int[1][];
						InfTree.inflate_trees_fixed(array, array2, array3, array4, _codec);
						codes.Init(array[0], array2[0], array3[0], 0, array4[0], 0);
						num3 >>= 3;
						i -= 3;
						mode = InflateBlockMode.CODES;
						break;
					}
					case 2u:
						num3 >>= 3;
						i -= 3;
						mode = InflateBlockMode.TABLE;
						break;
					case 3u:
						num3 >>= 3;
						i -= 3;
						mode = InflateBlockMode.BAD;
						_codec.Message = "invalid block type";
						r = -3;
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					break;
				}
				case InflateBlockMode.LENS:
					for (; i < 32; i += 8)
					{
						if (num2 == 0)
						{
							bitb = num3;
							bitk = i;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num4;
							return Flush(r);
						}
						r = 0;
						num2--;
						num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
					}
					if (((~num3 >> 16) & 0xFFFF) != (num3 & 0xFFFF))
					{
						mode = InflateBlockMode.BAD;
						_codec.Message = "invalid stored block lengths";
						r = -3;
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					left = (num3 & 0xFFFF);
					num3 = (i = 0);
					mode = ((left != 0) ? InflateBlockMode.STORED : ((last != 0) ? InflateBlockMode.DRY : InflateBlockMode.TYPE));
					break;
				case InflateBlockMode.STORED:
				{
					if (num2 == 0)
					{
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					if (num5 == 0)
					{
						if (num4 == end && readAt != 0)
						{
							num4 = 0;
							num5 = ((num4 >= readAt) ? (end - num4) : (readAt - num4 - 1));
						}
						if (num5 == 0)
						{
							writeAt = num4;
							r = Flush(r);
							num4 = writeAt;
							num5 = ((num4 >= readAt) ? (end - num4) : (readAt - num4 - 1));
							if (num4 == end && readAt != 0)
							{
								num4 = 0;
								num5 = ((num4 >= readAt) ? (end - num4) : (readAt - num4 - 1));
							}
							if (num5 == 0)
							{
								bitb = num3;
								bitk = i;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num4;
								return Flush(r);
							}
						}
					}
					r = 0;
					int num7 = left;
					if (num7 > num2)
					{
						num7 = num2;
					}
					if (num7 > num5)
					{
						num7 = num5;
					}
					Array.Copy(_codec.InputBuffer, num, window, num4, num7);
					num += num7;
					num2 -= num7;
					num4 += num7;
					num5 -= num7;
					if ((left -= num7) == 0)
					{
						mode = ((last != 0) ? InflateBlockMode.DRY : InflateBlockMode.TYPE);
					}
					break;
				}
				case InflateBlockMode.TABLE:
				{
					for (; i < 14; i += 8)
					{
						if (num2 == 0)
						{
							bitb = num3;
							bitk = i;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num4;
							return Flush(r);
						}
						r = 0;
						num2--;
						num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
					}
					int num7 = table = (num3 & 0x3FFF);
					if ((num7 & 0x1F) > 29 || ((num7 >> 5) & 0x1F) > 29)
					{
						mode = InflateBlockMode.BAD;
						_codec.Message = "too many length or distance symbols";
						r = -3;
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					num7 = 258 + (num7 & 0x1F) + ((num7 >> 5) & 0x1F);
					if (blens == null || blens.Length < num7)
					{
						blens = new int[num7];
					}
					else
					{
						Array.Clear(blens, 0, num7);
					}
					num3 >>= 14;
					i -= 14;
					index = 0;
					mode = InflateBlockMode.BTREE;
					goto case InflateBlockMode.BTREE;
				}
				case InflateBlockMode.BTREE:
				{
					while (index < 4 + (table >> 10))
					{
						for (; i < 3; i += 8)
						{
							if (num2 == 0)
							{
								bitb = num3;
								bitk = i;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num4;
								return Flush(r);
							}
							r = 0;
							num2--;
							num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
						}
						blens[border[index++]] = (num3 & 7);
						num3 >>= 3;
						i -= 3;
					}
					while (index < 19)
					{
						blens[border[index++]] = 0;
					}
					bb[0] = 7;
					int num7 = inftree.inflate_trees_bits(blens, bb, tb, hufts, _codec);
					if (num7 != 0)
					{
						r = num7;
						if (r == -3)
						{
							blens = null;
							mode = InflateBlockMode.BAD;
						}
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					index = 0;
					mode = InflateBlockMode.DTREE;
					goto case InflateBlockMode.DTREE;
				}
				case InflateBlockMode.DTREE:
				{
					int num7;
					while (true)
					{
						num7 = table;
						if (index >= 258 + (num7 & 0x1F) + ((num7 >> 5) & 0x1F))
						{
							break;
						}
						for (num7 = bb[0]; i < num7; i += 8)
						{
							if (num2 == 0)
							{
								bitb = num3;
								bitk = i;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num4;
								return Flush(r);
							}
							r = 0;
							num2--;
							num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
						}
						num7 = hufts[(tb[0] + (num3 & InternalInflateConstants.InflateMask[num7])) * 3 + 1];
						int num11 = hufts[(tb[0] + (num3 & InternalInflateConstants.InflateMask[num7])) * 3 + 2];
						if (num11 < 16)
						{
							num3 >>= num7;
							i -= num7;
							blens[index++] = num11;
						}
						else
						{
							int num12 = (num11 != 18) ? (num11 - 14) : 7;
							int num13 = (num11 != 18) ? 3 : 11;
							for (; i < num7 + num12; i += 8)
							{
								if (num2 == 0)
								{
									bitb = num3;
									bitk = i;
									_codec.AvailableBytesIn = num2;
									_codec.TotalBytesIn += num - _codec.NextIn;
									_codec.NextIn = num;
									writeAt = num4;
									return Flush(r);
								}
								r = 0;
								num2--;
								num3 |= (_codec.InputBuffer[num++] & 0xFF) << i;
							}
							num3 >>= num7;
							i -= num7;
							num13 += (num3 & InternalInflateConstants.InflateMask[num12]);
							num3 >>= num12;
							i -= num12;
							num12 = index;
							num7 = table;
							if (num12 + num13 > 258 + (num7 & 0x1F) + ((num7 >> 5) & 0x1F) || (num11 == 16 && num12 < 1))
							{
								blens = null;
								mode = InflateBlockMode.BAD;
								_codec.Message = "invalid bit length repeat";
								r = -3;
								bitb = num3;
								bitk = i;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num4;
								return Flush(r);
							}
							num11 = ((num11 == 16) ? blens[num12 - 1] : 0);
							do
							{
								blens[num12++] = num11;
							}
							while (--num13 != 0);
							index = num12;
						}
					}
					tb[0] = -1;
					int[] array5 = new int[1]
					{
						9
					};
					int[] array6 = new int[1]
					{
						6
					};
					int[] array7 = new int[1];
					int[] array8 = new int[1];
					num7 = table;
					num7 = inftree.inflate_trees_dynamic(257 + (num7 & 0x1F), 1 + ((num7 >> 5) & 0x1F), blens, array5, array6, array7, array8, hufts, _codec);
					switch (num7)
					{
					case -3:
						blens = null;
						mode = InflateBlockMode.BAD;
						goto default;
					default:
						r = num7;
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					case 0:
						break;
					}
					codes.Init(array5[0], array6[0], hufts, array7[0], hufts, array8[0]);
					mode = InflateBlockMode.CODES;
					goto case InflateBlockMode.CODES;
				}
				case InflateBlockMode.CODES:
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					r = codes.Process(this, r);
					if (r != 1)
					{
						return Flush(r);
					}
					r = 0;
					num = _codec.NextIn;
					num2 = _codec.AvailableBytesIn;
					num3 = bitb;
					i = bitk;
					num4 = writeAt;
					num5 = ((num4 >= readAt) ? (end - num4) : (readAt - num4 - 1));
					if (last == 0)
					{
						mode = InflateBlockMode.TYPE;
						break;
					}
					mode = InflateBlockMode.DRY;
					goto case InflateBlockMode.DRY;
				case InflateBlockMode.DRY:
					writeAt = num4;
					r = Flush(r);
					num4 = writeAt;
					num5 = ((num4 >= readAt) ? (end - num4) : (readAt - num4 - 1));
					if (readAt != writeAt)
					{
						bitb = num3;
						bitk = i;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num4;
						return Flush(r);
					}
					mode = InflateBlockMode.DONE;
					goto case InflateBlockMode.DONE;
				case InflateBlockMode.DONE:
					r = 1;
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				case InflateBlockMode.BAD:
					r = -3;
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				default:
					r = -2;
					bitb = num3;
					bitk = i;
					_codec.AvailableBytesIn = num2;
					_codec.TotalBytesIn += num - _codec.NextIn;
					_codec.NextIn = num;
					writeAt = num4;
					return Flush(r);
				}
			}
		}

		internal void Free()
		{
			Reset();
			window = null;
			hufts = null;
		}

		internal void SetDictionary(byte[] d, int start, int n)
		{
			Array.Copy(d, start, window, 0, n);
			readAt = (writeAt = n);
		}

		internal int SyncPoint()
		{
			return (mode == InflateBlockMode.LENS) ? 1 : 0;
		}

		internal int Flush(int r)
		{
			for (int i = 0; i < 2; i++)
			{
				int num = (i != 0) ? (writeAt - readAt) : (((readAt > writeAt) ? end : writeAt) - readAt);
				if (num == 0)
				{
					if (r == -5)
					{
						r = 0;
					}
					return r;
				}
				if (num > _codec.AvailableBytesOut)
				{
					num = _codec.AvailableBytesOut;
				}
				if (num != 0 && r == -5)
				{
					r = 0;
				}
				_codec.AvailableBytesOut -= num;
				_codec.TotalBytesOut += num;
				if (checkfn != null)
				{
					_codec._Adler32 = (check = Adler.Adler32(check, window, readAt, num));
				}
				Array.Copy(window, readAt, _codec.OutputBuffer, _codec.NextOut, num);
				_codec.NextOut += num;
				readAt += num;
				if (readAt == end && i == 0)
				{
					readAt = 0;
					if (writeAt == end)
					{
						writeAt = 0;
					}
				}
				else
				{
					i++;
				}
			}
			return r;
		}
	}
}
