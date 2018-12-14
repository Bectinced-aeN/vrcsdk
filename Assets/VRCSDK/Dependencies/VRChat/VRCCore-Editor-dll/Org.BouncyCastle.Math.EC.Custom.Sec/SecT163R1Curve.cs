using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT163R1Curve : AbstractF2mCurve
	{
		private const int SecT163R1_DEFAULT_COORDS = 6;

		protected readonly SecT163R1Point m_infinity;

		public override ECPoint Infinity => m_infinity;

		public override int FieldSize => 163;

		public override bool IsKoblitz => false;

		public virtual int M => 163;

		public virtual bool IsTrinomial => false;

		public virtual int K1 => 3;

		public virtual int K2 => 6;

		public virtual int K3 => 7;

		public SecT163R1Curve()
			: base(163, 3, 6, 7)
		{
			m_infinity = new SecT163R1Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("07B6882CAAEFA84F9554FF8428BD88E246D2782AE2")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("0713612DCDDCB40AAB946BDA29CA91F73AF958AFD9")));
			m_order = new BigInteger(1, Hex.Decode("03FFFFFFFFFFFFFFFFFFFF48AAB689C29CA710279B"));
			m_cofactor = BigInteger.Two;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT163R1Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			if (coord == 6)
			{
				return true;
			}
			return false;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecT163FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT163R1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT163R1Point(this, x, y, zs, withCompression);
		}

		protected override ECPoint DecompressPoint(int yTilde, BigInteger X1)
		{
			ECFieldElement eCFieldElement = FromBigInteger(X1);
			ECFieldElement eCFieldElement2 = null;
			if (eCFieldElement.IsZero)
			{
				eCFieldElement2 = B.Sqrt();
			}
			else
			{
				ECFieldElement beta = eCFieldElement.Square().Invert().Multiply(B)
					.Add(A)
					.Add(eCFieldElement);
				ECFieldElement eCFieldElement3 = SolveQuadraticEquation(beta);
				if (eCFieldElement3 != null)
				{
					if (eCFieldElement3.TestBitZero() != (yTilde == 1))
					{
						eCFieldElement3 = eCFieldElement3.AddOne();
					}
					int coordinateSystem = CoordinateSystem;
					eCFieldElement2 = ((coordinateSystem != 5 && coordinateSystem != 6) ? eCFieldElement3.Multiply(eCFieldElement) : eCFieldElement3.Add(eCFieldElement));
				}
			}
			if (eCFieldElement2 == null)
			{
				throw new ArgumentException("Invalid point compression");
			}
			return CreateRawPoint(eCFieldElement, eCFieldElement2, withCompression: true);
		}

		private ECFieldElement SolveQuadraticEquation(ECFieldElement beta)
		{
			if (beta.IsZero)
			{
				return beta;
			}
			ECFieldElement eCFieldElement = FromBigInteger(BigInteger.Zero);
			ECFieldElement eCFieldElement2 = null;
			ECFieldElement eCFieldElement3 = null;
			Random random = new Random();
			do
			{
				ECFieldElement b = FromBigInteger(new BigInteger(163, random));
				eCFieldElement2 = eCFieldElement;
				ECFieldElement eCFieldElement4 = beta;
				for (int i = 1; i < 163; i++)
				{
					ECFieldElement eCFieldElement5 = eCFieldElement4.Square();
					eCFieldElement2 = eCFieldElement2.Square().Add(eCFieldElement5.Multiply(b));
					eCFieldElement4 = eCFieldElement5.Add(beta);
				}
				if (!eCFieldElement4.IsZero)
				{
					return null;
				}
				eCFieldElement3 = eCFieldElement2.Square().Add(eCFieldElement2);
			}
			while (eCFieldElement3.IsZero);
			return eCFieldElement2;
		}
	}
}
