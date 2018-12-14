using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities.Encoders;
using System;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT239K1Curve : AbstractF2mCurve
	{
		private const int SecT239K1_DEFAULT_COORDS = 6;

		protected readonly SecT239K1Point m_infinity;

		public override ECPoint Infinity => m_infinity;

		public override int FieldSize => 239;

		public override bool IsKoblitz => true;

		public virtual int M => 239;

		public virtual bool IsTrinomial => true;

		public virtual int K1 => 158;

		public virtual int K2 => 0;

		public virtual int K3 => 0;

		public SecT239K1Curve()
			: base(239, 158, 0, 0)
		{
			m_infinity = new SecT239K1Point(this, null, null);
			m_a = FromBigInteger(BigInteger.Zero);
			m_b = FromBigInteger(BigInteger.One);
			m_order = new BigInteger(1, Hex.Decode("2000000000000000000000000000005A79FEC67CB6E91F1C1DA800E478A5"));
			m_cofactor = BigInteger.ValueOf(4L);
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT239K1Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			if (coord == 6)
			{
				return true;
			}
			return false;
		}

		protected override ECMultiplier CreateDefaultMultiplier()
		{
			return new WTauNafMultiplier();
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecT239FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT239K1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT239K1Point(this, x, y, zs, withCompression);
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
				ECFieldElement b = FromBigInteger(new BigInteger(239, random));
				eCFieldElement2 = eCFieldElement;
				ECFieldElement eCFieldElement4 = beta;
				for (int i = 1; i < 239; i++)
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
