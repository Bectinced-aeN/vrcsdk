using System;

namespace Org.BouncyCastle.Utilities.Zlib
{
	internal sealed class InfCodes
	{
		private const int Z_OK = 0;

		private const int Z_STREAM_END = 1;

		private const int Z_NEED_DICT = 2;

		private const int Z_ERRNO = -1;

		private const int Z_STREAM_ERROR = -2;

		private const int Z_DATA_ERROR = -3;

		private const int Z_MEM_ERROR = -4;

		private const int Z_BUF_ERROR = -5;

		private const int Z_VERSION_ERROR = -6;

		private const int START = 0;

		private const int LEN = 1;

		private const int LENEXT = 2;

		private const int DIST = 3;

		private const int DISTEXT = 4;

		private const int COPY = 5;

		private const int LIT = 6;

		private const int WASH = 7;

		private const int END = 8;

		private const int BADCODE = 9;

		private static readonly int[] inflate_mask = new int[17]
		{
			0,
			1,
			3,
			7,
			15,
			31,
			63,
			127,
			255,
			511,
			1023,
			2047,
			4095,
			8191,
			16383,
			32767,
			65535
		};

		private int mode;

		private int len;

		private int[] tree;

		private int tree_index;

		private int need;

		private int lit;

		private int get;

		private int dist;

		private byte lbits;

		private byte dbits;

		private int[] ltree;

		private int ltree_index;

		private int[] dtree;

		private int dtree_index;

		internal InfCodes()
		{
		}

		internal void init(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, ZStream z)
		{
			mode = 0;
			lbits = (byte)bl;
			dbits = (byte)bd;
			ltree = tl;
			ltree_index = tl_index;
			dtree = td;
			dtree_index = td_index;
			tree = null;
		}

		internal int proc(InfBlocks s, ZStream z, int r)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			num3 = z.next_in_index;
			int num4 = z.avail_in;
			num = s.bitb;
			num2 = s.bitk;
			int num5 = s.write;
			int num6 = (num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1);
			while (true)
			{
				switch (mode)
				{
				case 0:
					if (num6 >= 258 && num4 >= 10)
					{
						s.bitb = num;
						s.bitk = num2;
						z.avail_in = num4;
						z.total_in += num3 - z.next_in_index;
						z.next_in_index = num3;
						s.write = num5;
						r = inflate_fast(lbits, dbits, ltree, ltree_index, dtree, dtree_index, s, z);
						num3 = z.next_in_index;
						num4 = z.avail_in;
						num = s.bitb;
						num2 = s.bitk;
						num5 = s.write;
						num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
						if (r != 0)
						{
							mode = ((r != 1) ? 9 : 7);
							break;
						}
					}
					need = lbits;
					tree = ltree;
					tree_index = ltree_index;
					mode = 1;
					goto case 1;
				case 1:
				{
					int num8;
					for (num8 = need; num2 < num8; num2 += 8)
					{
						if (num4 == 0)
						{
							s.bitb = num;
							s.bitk = num2;
							z.avail_in = num4;
							z.total_in += num3 - z.next_in_index;
							z.next_in_index = num3;
							s.write = num5;
							return s.inflate_flush(z, r);
						}
						r = 0;
						num4--;
						num |= (z.next_in[num3++] & 0xFF) << num2;
					}
					int num11 = (tree_index + (num & inflate_mask[num8])) * 3;
					num >>= tree[num11 + 1];
					num2 -= tree[num11 + 1];
					int num12 = tree[num11];
					if (num12 == 0)
					{
						lit = tree[num11 + 2];
						mode = 6;
					}
					else if ((num12 & 0x10) != 0)
					{
						get = (num12 & 0xF);
						len = tree[num11 + 2];
						mode = 2;
					}
					else if ((num12 & 0x40) == 0)
					{
						need = num12;
						tree_index = num11 / 3 + tree[num11 + 2];
					}
					else
					{
						if ((num12 & 0x20) == 0)
						{
							mode = 9;
							z.msg = "invalid literal/length code";
							r = -3;
							s.bitb = num;
							s.bitk = num2;
							z.avail_in = num4;
							z.total_in += num3 - z.next_in_index;
							z.next_in_index = num3;
							s.write = num5;
							return s.inflate_flush(z, r);
						}
						mode = 7;
					}
					break;
				}
				case 2:
				{
					int num8;
					for (num8 = get; num2 < num8; num2 += 8)
					{
						if (num4 == 0)
						{
							s.bitb = num;
							s.bitk = num2;
							z.avail_in = num4;
							z.total_in += num3 - z.next_in_index;
							z.next_in_index = num3;
							s.write = num5;
							return s.inflate_flush(z, r);
						}
						r = 0;
						num4--;
						num |= (z.next_in[num3++] & 0xFF) << num2;
					}
					len += (num & inflate_mask[num8]);
					num >>= num8;
					num2 -= num8;
					need = dbits;
					tree = dtree;
					tree_index = dtree_index;
					mode = 3;
					goto case 3;
				}
				case 3:
				{
					int num8;
					for (num8 = need; num2 < num8; num2 += 8)
					{
						if (num4 == 0)
						{
							s.bitb = num;
							s.bitk = num2;
							z.avail_in = num4;
							z.total_in += num3 - z.next_in_index;
							z.next_in_index = num3;
							s.write = num5;
							return s.inflate_flush(z, r);
						}
						r = 0;
						num4--;
						num |= (z.next_in[num3++] & 0xFF) << num2;
					}
					int num11 = (tree_index + (num & inflate_mask[num8])) * 3;
					num >>= tree[num11 + 1];
					num2 -= tree[num11 + 1];
					int num12 = tree[num11];
					if ((num12 & 0x10) != 0)
					{
						get = (num12 & 0xF);
						dist = tree[num11 + 2];
						mode = 4;
					}
					else
					{
						if ((num12 & 0x40) != 0)
						{
							mode = 9;
							z.msg = "invalid distance code";
							r = -3;
							s.bitb = num;
							s.bitk = num2;
							z.avail_in = num4;
							z.total_in += num3 - z.next_in_index;
							z.next_in_index = num3;
							s.write = num5;
							return s.inflate_flush(z, r);
						}
						need = num12;
						tree_index = num11 / 3 + tree[num11 + 2];
					}
					break;
				}
				case 4:
				{
					int num8;
					for (num8 = get; num2 < num8; num2 += 8)
					{
						if (num4 == 0)
						{
							s.bitb = num;
							s.bitk = num2;
							z.avail_in = num4;
							z.total_in += num3 - z.next_in_index;
							z.next_in_index = num3;
							s.write = num5;
							return s.inflate_flush(z, r);
						}
						r = 0;
						num4--;
						num |= (z.next_in[num3++] & 0xFF) << num2;
					}
					dist += (num & inflate_mask[num8]);
					num >>= num8;
					num2 -= num8;
					mode = 5;
					goto case 5;
				}
				case 5:
				{
					int i;
					for (i = num5 - dist; i < 0; i += s.end)
					{
					}
					while (len != 0)
					{
						if (num6 == 0)
						{
							if (num5 == s.end && s.read != 0)
							{
								num5 = 0;
								num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
							}
							if (num6 == 0)
							{
								s.write = num5;
								r = s.inflate_flush(z, r);
								num5 = s.write;
								num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
								if (num5 == s.end && s.read != 0)
								{
									num5 = 0;
									num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
								}
								if (num6 == 0)
								{
									s.bitb = num;
									s.bitk = num2;
									z.avail_in = num4;
									z.total_in += num3 - z.next_in_index;
									z.next_in_index = num3;
									s.write = num5;
									return s.inflate_flush(z, r);
								}
							}
						}
						s.window[num5++] = s.window[i++];
						num6--;
						if (i == s.end)
						{
							i = 0;
						}
						len--;
					}
					mode = 0;
					break;
				}
				case 6:
					if (num6 == 0)
					{
						if (num5 == s.end && s.read != 0)
						{
							num5 = 0;
							num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
						}
						if (num6 == 0)
						{
							s.write = num5;
							r = s.inflate_flush(z, r);
							num5 = s.write;
							num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
							if (num5 == s.end && s.read != 0)
							{
								num5 = 0;
								num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
							}
							if (num6 == 0)
							{
								s.bitb = num;
								s.bitk = num2;
								z.avail_in = num4;
								z.total_in += num3 - z.next_in_index;
								z.next_in_index = num3;
								s.write = num5;
								return s.inflate_flush(z, r);
							}
						}
					}
					r = 0;
					s.window[num5++] = (byte)lit;
					num6--;
					mode = 0;
					break;
				case 7:
					if (num2 > 7)
					{
						num2 -= 8;
						num4++;
						num3--;
					}
					s.write = num5;
					r = s.inflate_flush(z, r);
					num5 = s.write;
					num6 = ((num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1));
					if (s.read != s.write)
					{
						s.bitb = num;
						s.bitk = num2;
						z.avail_in = num4;
						z.total_in += num3 - z.next_in_index;
						z.next_in_index = num3;
						s.write = num5;
						return s.inflate_flush(z, r);
					}
					mode = 8;
					goto case 8;
				case 8:
					r = 1;
					s.bitb = num;
					s.bitk = num2;
					z.avail_in = num4;
					z.total_in += num3 - z.next_in_index;
					z.next_in_index = num3;
					s.write = num5;
					return s.inflate_flush(z, r);
				case 9:
					r = -3;
					s.bitb = num;
					s.bitk = num2;
					z.avail_in = num4;
					z.total_in += num3 - z.next_in_index;
					z.next_in_index = num3;
					s.write = num5;
					return s.inflate_flush(z, r);
				default:
					r = -2;
					s.bitb = num;
					s.bitk = num2;
					z.avail_in = num4;
					z.total_in += num3 - z.next_in_index;
					z.next_in_index = num3;
					s.write = num5;
					return s.inflate_flush(z, r);
				}
			}
		}

		internal void free(ZStream z)
		{
		}

		internal int inflate_fast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InfBlocks s, ZStream z)
		{
			int num = z.next_in_index;
			int num2 = z.avail_in;
			int num3 = s.bitb;
			int num4 = s.bitk;
			int num5 = s.write;
			int num6 = (num5 >= s.read) ? (s.end - num5) : (s.read - num5 - 1);
			int num7 = inflate_mask[bl];
			int num8 = inflate_mask[bd];
			int num13;
			while (true)
			{
				if (num4 < 20)
				{
					num2--;
					num3 |= (z.next_in[num++] & 0xFF) << (num4 & 0x1F);
					num4 += 8;
				}
				else
				{
					int num10 = num3 & num7;
					int num11 = (tl_index + num10) * 3;
					int num12;
					if ((num12 = tl[num11]) != 0)
					{
						while (true)
						{
							num3 >>= tl[num11 + 1];
							num4 -= tl[num11 + 1];
							if ((num12 & 0x10) != 0)
							{
								num12 &= 0xF;
								num13 = tl[num11 + 2] + (num3 & inflate_mask[num12]);
								num3 >>= num12;
								for (num4 -= num12; num4 < 15; num4 += 8)
								{
									num2--;
									num3 |= (z.next_in[num++] & 0xFF) << num4;
								}
								num10 = (num3 & num8);
								num11 = (td_index + num10) * 3;
								num12 = td[num11];
								while (true)
								{
									num3 >>= td[num11 + 1];
									num4 -= td[num11 + 1];
									if ((num12 & 0x10) != 0)
									{
										break;
									}
									if ((num12 & 0x40) != 0)
									{
										z.msg = "invalid distance code";
										num13 = z.avail_in - num2;
										num13 = ((num4 >> 3 >= num13) ? num13 : (num4 >> 3));
										num2 += num13;
										num -= num13;
										num4 -= num13 << 3;
										s.bitb = num3;
										s.bitk = num4;
										z.avail_in = num2;
										z.total_in += num - z.next_in_index;
										z.next_in_index = num;
										s.write = num5;
										return -3;
									}
									num10 += td[num11 + 2];
									num10 += (num3 & inflate_mask[num12]);
									num11 = (td_index + num10) * 3;
									num12 = td[num11];
								}
								for (num12 &= 0xF; num4 < num12; num4 += 8)
								{
									num2--;
									num3 |= (z.next_in[num++] & 0xFF) << num4;
								}
								int num16 = td[num11 + 2] + (num3 & inflate_mask[num12]);
								num3 >>= num12;
								num4 -= num12;
								num6 -= num13;
								int num17;
								if (num5 >= num16)
								{
									num17 = num5 - num16;
									if (num5 - num17 > 0 && 2 > num5 - num17)
									{
										s.window[num5++] = s.window[num17++];
										s.window[num5++] = s.window[num17++];
										num13 -= 2;
									}
									else
									{
										Array.Copy(s.window, num17, s.window, num5, 2);
										num5 += 2;
										num17 += 2;
										num13 -= 2;
									}
								}
								else
								{
									num17 = num5 - num16;
									do
									{
										num17 += s.end;
									}
									while (num17 < 0);
									num12 = s.end - num17;
									if (num13 > num12)
									{
										num13 -= num12;
										if (num5 - num17 > 0 && num12 > num5 - num17)
										{
											do
											{
												s.window[num5++] = s.window[num17++];
											}
											while (--num12 != 0);
										}
										else
										{
											Array.Copy(s.window, num17, s.window, num5, num12);
											num5 += num12;
											num17 += num12;
											num12 = 0;
										}
										num17 = 0;
									}
								}
								if (num5 - num17 > 0 && num13 > num5 - num17)
								{
									do
									{
										s.window[num5++] = s.window[num17++];
									}
									while (--num13 != 0);
								}
								else
								{
									Array.Copy(s.window, num17, s.window, num5, num13);
									num5 += num13;
									num17 += num13;
									num13 = 0;
								}
								break;
							}
							if ((num12 & 0x40) != 0)
							{
								if ((num12 & 0x20) != 0)
								{
									num13 = z.avail_in - num2;
									num13 = ((num4 >> 3 >= num13) ? num13 : (num4 >> 3));
									num2 += num13;
									num -= num13;
									num4 -= num13 << 3;
									s.bitb = num3;
									s.bitk = num4;
									z.avail_in = num2;
									z.total_in += num - z.next_in_index;
									z.next_in_index = num;
									s.write = num5;
									return 1;
								}
								z.msg = "invalid literal/length code";
								num13 = z.avail_in - num2;
								num13 = ((num4 >> 3 >= num13) ? num13 : (num4 >> 3));
								num2 += num13;
								num -= num13;
								num4 -= num13 << 3;
								s.bitb = num3;
								s.bitk = num4;
								z.avail_in = num2;
								z.total_in += num - z.next_in_index;
								z.next_in_index = num;
								s.write = num5;
								return -3;
							}
							num10 += tl[num11 + 2];
							num10 += (num3 & inflate_mask[num12]);
							num11 = (tl_index + num10) * 3;
							if ((num12 = tl[num11]) == 0)
							{
								num3 >>= tl[num11 + 1];
								num4 -= tl[num11 + 1];
								s.window[num5++] = (byte)tl[num11 + 2];
								num6--;
								break;
							}
						}
					}
					else
					{
						num3 >>= tl[num11 + 1];
						num4 -= tl[num11 + 1];
						s.window[num5++] = (byte)tl[num11 + 2];
						num6--;
					}
					if (num6 < 258 || num2 < 10)
					{
						break;
					}
				}
			}
			num13 = z.avail_in - num2;
			num13 = ((num4 >> 3 >= num13) ? num13 : (num4 >> 3));
			num2 += num13;
			num -= num13;
			num4 -= num13 << 3;
			s.bitb = num3;
			s.bitk = num4;
			z.avail_in = num2;
			z.total_in += num - z.next_in_index;
			z.next_in_index = num;
			s.write = num5;
			return 0;
		}
	}
}
