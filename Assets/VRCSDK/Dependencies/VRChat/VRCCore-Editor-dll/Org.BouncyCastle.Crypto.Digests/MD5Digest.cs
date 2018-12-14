using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	internal class MD5Digest : GeneralDigest
	{
		private const int DigestLength = 16;

		private uint H1;

		private uint H2;

		private uint H3;

		private uint H4;

		private uint[] X = new uint[16];

		private int xOff;

		private static readonly int S11 = 7;

		private static readonly int S12 = 12;

		private static readonly int S13 = 17;

		private static readonly int S14 = 22;

		private static readonly int S21 = 5;

		private static readonly int S22 = 9;

		private static readonly int S23 = 14;

		private static readonly int S24 = 20;

		private static readonly int S31 = 4;

		private static readonly int S32 = 11;

		private static readonly int S33 = 16;

		private static readonly int S34 = 23;

		private static readonly int S41 = 6;

		private static readonly int S42 = 10;

		private static readonly int S43 = 15;

		private static readonly int S44 = 21;

		public override string AlgorithmName => "MD5";

		public MD5Digest()
		{
			Reset();
		}

		public MD5Digest(MD5Digest t)
			: base(t)
		{
			CopyIn(t);
		}

		private void CopyIn(MD5Digest t)
		{
			CopyIn((GeneralDigest)t);
			H1 = t.H1;
			H2 = t.H2;
			H3 = t.H3;
			H4 = t.H4;
			Array.Copy(t.X, 0, X, 0, t.X.Length);
			xOff = t.xOff;
		}

		public override int GetDigestSize()
		{
			return 16;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			X[xOff] = Pack.LE_To_UInt32(input, inOff);
			if (++xOff == 16)
			{
				ProcessBlock();
			}
		}

		internal override void ProcessLength(long bitLength)
		{
			if (xOff > 14)
			{
				if (xOff == 15)
				{
					X[15] = 0u;
				}
				ProcessBlock();
			}
			for (int i = xOff; i < 14; i++)
			{
				X[i] = 0u;
			}
			X[14] = (uint)bitLength;
			X[15] = (uint)((ulong)bitLength >> 32);
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			Finish();
			Pack.UInt32_To_LE(H1, output, outOff);
			Pack.UInt32_To_LE(H2, output, outOff + 4);
			Pack.UInt32_To_LE(H3, output, outOff + 8);
			Pack.UInt32_To_LE(H4, output, outOff + 12);
			Reset();
			return 16;
		}

		public override void Reset()
		{
			base.Reset();
			H1 = 1732584193u;
			H2 = 4023233417u;
			H3 = 2562383102u;
			H4 = 271733878u;
			xOff = 0;
			for (int i = 0; i != X.Length; i++)
			{
				X[i] = 0u;
			}
		}

		private static uint RotateLeft(uint x, int n)
		{
			return (x << n) | (x >> 32 - n);
		}

		private static uint F(uint u, uint v, uint w)
		{
			return (u & v) | (~u & w);
		}

		private static uint G(uint u, uint v, uint w)
		{
			return (u & w) | (v & ~w);
		}

		private static uint H(uint u, uint v, uint w)
		{
			return u ^ v ^ w;
		}

		private static uint K(uint u, uint v, uint w)
		{
			return v ^ (u | ~w);
		}

		internal override void ProcessBlock()
		{
			uint h = H1;
			uint h2 = H2;
			uint h3 = H3;
			uint h4 = H4;
			h = RotateLeft((uint)((int)(h + F(h2, h3, h4) + X[0]) + -680876936), S11) + h2;
			h4 = RotateLeft((uint)((int)(h4 + F(h, h2, h3) + X[1]) + -389564586), S12) + h;
			h3 = RotateLeft(h3 + F(h4, h, h2) + X[2] + 606105819, S13) + h4;
			h2 = RotateLeft((uint)((int)(h2 + F(h3, h4, h) + X[3]) + -1044525330), S14) + h3;
			h = RotateLeft((uint)((int)(h + F(h2, h3, h4) + X[4]) + -176418897), S11) + h2;
			h4 = RotateLeft(h4 + F(h, h2, h3) + X[5] + 1200080426, S12) + h;
			h3 = RotateLeft((uint)((int)(h3 + F(h4, h, h2) + X[6]) + -1473231341), S13) + h4;
			h2 = RotateLeft((uint)((int)(h2 + F(h3, h4, h) + X[7]) + -45705983), S14) + h3;
			h = RotateLeft(h + F(h2, h3, h4) + X[8] + 1770035416, S11) + h2;
			h4 = RotateLeft((uint)((int)(h4 + F(h, h2, h3) + X[9]) + -1958414417), S12) + h;
			h3 = RotateLeft((uint)((int)(h3 + F(h4, h, h2) + X[10]) + -42063), S13) + h4;
			h2 = RotateLeft((uint)((int)(h2 + F(h3, h4, h) + X[11]) + -1990404162), S14) + h3;
			h = RotateLeft(h + F(h2, h3, h4) + X[12] + 1804603682, S11) + h2;
			h4 = RotateLeft((uint)((int)(h4 + F(h, h2, h3) + X[13]) + -40341101), S12) + h;
			h3 = RotateLeft((uint)((int)(h3 + F(h4, h, h2) + X[14]) + -1502002290), S13) + h4;
			h2 = RotateLeft(h2 + F(h3, h4, h) + X[15] + 1236535329, S14) + h3;
			h = RotateLeft((uint)((int)(h + G(h2, h3, h4) + X[1]) + -165796510), S21) + h2;
			h4 = RotateLeft((uint)((int)(h4 + G(h, h2, h3) + X[6]) + -1069501632), S22) + h;
			h3 = RotateLeft(h3 + G(h4, h, h2) + X[11] + 643717713, S23) + h4;
			h2 = RotateLeft((uint)((int)(h2 + G(h3, h4, h) + X[0]) + -373897302), S24) + h3;
			h = RotateLeft((uint)((int)(h + G(h2, h3, h4) + X[5]) + -701558691), S21) + h2;
			h4 = RotateLeft(h4 + G(h, h2, h3) + X[10] + 38016083, S22) + h;
			h3 = RotateLeft((uint)((int)(h3 + G(h4, h, h2) + X[15]) + -660478335), S23) + h4;
			h2 = RotateLeft((uint)((int)(h2 + G(h3, h4, h) + X[4]) + -405537848), S24) + h3;
			h = RotateLeft(h + G(h2, h3, h4) + X[9] + 568446438, S21) + h2;
			h4 = RotateLeft((uint)((int)(h4 + G(h, h2, h3) + X[14]) + -1019803690), S22) + h;
			h3 = RotateLeft((uint)((int)(h3 + G(h4, h, h2) + X[3]) + -187363961), S23) + h4;
			h2 = RotateLeft(h2 + G(h3, h4, h) + X[8] + 1163531501, S24) + h3;
			h = RotateLeft((uint)((int)(h + G(h2, h3, h4) + X[13]) + -1444681467), S21) + h2;
			h4 = RotateLeft((uint)((int)(h4 + G(h, h2, h3) + X[2]) + -51403784), S22) + h;
			h3 = RotateLeft(h3 + G(h4, h, h2) + X[7] + 1735328473, S23) + h4;
			h2 = RotateLeft((uint)((int)(h2 + G(h3, h4, h) + X[12]) + -1926607734), S24) + h3;
			h = RotateLeft((uint)((int)(h + H(h2, h3, h4) + X[5]) + -378558), S31) + h2;
			h4 = RotateLeft((uint)((int)(h4 + H(h, h2, h3) + X[8]) + -2022574463), S32) + h;
			h3 = RotateLeft(h3 + H(h4, h, h2) + X[11] + 1839030562, S33) + h4;
			h2 = RotateLeft((uint)((int)(h2 + H(h3, h4, h) + X[14]) + -35309556), S34) + h3;
			h = RotateLeft((uint)((int)(h + H(h2, h3, h4) + X[1]) + -1530992060), S31) + h2;
			h4 = RotateLeft(h4 + H(h, h2, h3) + X[4] + 1272893353, S32) + h;
			h3 = RotateLeft((uint)((int)(h3 + H(h4, h, h2) + X[7]) + -155497632), S33) + h4;
			h2 = RotateLeft((uint)((int)(h2 + H(h3, h4, h) + X[10]) + -1094730640), S34) + h3;
			h = RotateLeft(h + H(h2, h3, h4) + X[13] + 681279174, S31) + h2;
			h4 = RotateLeft((uint)((int)(h4 + H(h, h2, h3) + X[0]) + -358537222), S32) + h;
			h3 = RotateLeft((uint)((int)(h3 + H(h4, h, h2) + X[3]) + -722521979), S33) + h4;
			h2 = RotateLeft(h2 + H(h3, h4, h) + X[6] + 76029189, S34) + h3;
			h = RotateLeft((uint)((int)(h + H(h2, h3, h4) + X[9]) + -640364487), S31) + h2;
			h4 = RotateLeft((uint)((int)(h4 + H(h, h2, h3) + X[12]) + -421815835), S32) + h;
			h3 = RotateLeft(h3 + H(h4, h, h2) + X[15] + 530742520, S33) + h4;
			h2 = RotateLeft((uint)((int)(h2 + H(h3, h4, h) + X[2]) + -995338651), S34) + h3;
			h = RotateLeft((uint)((int)(h + K(h2, h3, h4) + X[0]) + -198630844), S41) + h2;
			h4 = RotateLeft(h4 + K(h, h2, h3) + X[7] + 1126891415, S42) + h;
			h3 = RotateLeft((uint)((int)(h3 + K(h4, h, h2) + X[14]) + -1416354905), S43) + h4;
			h2 = RotateLeft((uint)((int)(h2 + K(h3, h4, h) + X[5]) + -57434055), S44) + h3;
			h = RotateLeft(h + K(h2, h3, h4) + X[12] + 1700485571, S41) + h2;
			h4 = RotateLeft((uint)((int)(h4 + K(h, h2, h3) + X[3]) + -1894986606), S42) + h;
			h3 = RotateLeft((uint)((int)(h3 + K(h4, h, h2) + X[10]) + -1051523), S43) + h4;
			h2 = RotateLeft((uint)((int)(h2 + K(h3, h4, h) + X[1]) + -2054922799), S44) + h3;
			h = RotateLeft(h + K(h2, h3, h4) + X[8] + 1873313359, S41) + h2;
			h4 = RotateLeft((uint)((int)(h4 + K(h, h2, h3) + X[15]) + -30611744), S42) + h;
			h3 = RotateLeft((uint)((int)(h3 + K(h4, h, h2) + X[6]) + -1560198380), S43) + h4;
			h2 = RotateLeft(h2 + K(h3, h4, h) + X[13] + 1309151649, S44) + h3;
			h = RotateLeft((uint)((int)(h + K(h2, h3, h4) + X[4]) + -145523070), S41) + h2;
			h4 = RotateLeft((uint)((int)(h4 + K(h, h2, h3) + X[11]) + -1120210379), S42) + h;
			h3 = RotateLeft(h3 + K(h4, h, h2) + X[2] + 718787259, S43) + h4;
			h2 = RotateLeft((uint)((int)(h2 + K(h3, h4, h) + X[9]) + -343485551), S44) + h3;
			H1 += h;
			H2 += h2;
			H3 += h3;
			H4 += h4;
			xOff = 0;
		}

		public override IMemoable Copy()
		{
			return new MD5Digest(this);
		}

		public override void Reset(IMemoable other)
		{
			MD5Digest t = (MD5Digest)other;
			CopyIn(t);
		}
	}
}
