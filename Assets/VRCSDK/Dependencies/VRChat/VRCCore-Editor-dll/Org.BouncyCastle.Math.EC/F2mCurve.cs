using Org.BouncyCastle.Math.EC.Multiplier;
using System;

namespace Org.BouncyCastle.Math.EC
{
	internal class F2mCurve : AbstractF2mCurve
	{
		private const int F2M_DEFAULT_COORDS = 6;

		private readonly int m;

		private readonly int k1;

		private readonly int k2;

		private readonly int k3;

		protected readonly F2mPoint m_infinity;

		public override int FieldSize => m;

		public override ECPoint Infinity => m_infinity;

		public int M => m;

		public int K1 => k1;

		public int K2 => k2;

		public int K3 => k3;

		[Obsolete("Use 'Order' property instead")]
		public BigInteger N
		{
			get
			{
				return m_order;
			}
		}

		[Obsolete("Use 'Cofactor' property instead")]
		public BigInteger H
		{
			get
			{
				return m_cofactor;
			}
		}

		public F2mCurve(int m, int k, BigInteger a, BigInteger b)
			: this(m, k, 0, 0, a, b, null, null)
		{
		}

		public F2mCurve(int m, int k, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor)
			: this(m, k, 0, 0, a, b, order, cofactor)
		{
		}

		public F2mCurve(int m, int k1, int k2, int k3, BigInteger a, BigInteger b)
			: this(m, k1, k2, k3, a, b, null, null)
		{
		}

		public F2mCurve(int m, int k1, int k2, int k3, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor)
			: base(m, k1, k2, k3)
		{
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
			m_order = order;
			m_cofactor = cofactor;
			m_infinity = new F2mPoint(this, null, null);
			if (k1 == 0)
			{
				throw new ArgumentException("k1 must be > 0");
			}
			if (k2 == 0)
			{
				if (k3 != 0)
				{
					throw new ArgumentException("k3 must be 0 if k2 == 0");
				}
			}
			else
			{
				if (k2 <= k1)
				{
					throw new ArgumentException("k2 must be > k1");
				}
				if (k3 <= k2)
				{
					throw new ArgumentException("k3 must be > k2");
				}
			}
			m_a = FromBigInteger(a);
			m_b = FromBigInteger(b);
			m_coord = 6;
		}

		protected F2mCurve(int m, int k1, int k2, int k3, ECFieldElement a, ECFieldElement b, BigInteger order, BigInteger cofactor)
			: base(m, k1, k2, k3)
		{
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
			m_order = order;
			m_cofactor = cofactor;
			m_infinity = new F2mPoint(this, null, null);
			m_a = a;
			m_b = b;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new F2mCurve(m, k1, k2, k3, m_a, m_b, m_order, m_cofactor);
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			if (coord == 0 || coord == 1 || coord == 6)
			{
				return true;
			}
			return false;
		}

		protected override ECMultiplier CreateDefaultMultiplier()
		{
			if (IsKoblitz)
			{
				return new WTauNafMultiplier();
			}
			return base.CreateDefaultMultiplier();
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new F2mFieldElement(m, k1, k2, k3, x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new F2mPoint(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new F2mPoint(this, x, y, zs, withCompression);
		}

		protected override ECPoint DecompressPoint(int yTilde, BigInteger X1)
		{
			ECFieldElement eCFieldElement = FromBigInteger(X1);
			ECFieldElement eCFieldElement2 = null;
			if (eCFieldElement.IsZero)
			{
				eCFieldElement2 = m_b.Sqrt();
			}
			else
			{
				ECFieldElement beta = eCFieldElement.Square().Invert().Multiply(B)
					.Add(A)
					.Add(eCFieldElement);
				ECFieldElement eCFieldElement3 = SolveQuadradicEquation(beta);
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

		private ECFieldElement SolveQuadradicEquation(ECFieldElement beta)
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
				ECFieldElement b = FromBigInteger(new BigInteger(m, random));
				eCFieldElement2 = eCFieldElement;
				ECFieldElement eCFieldElement4 = beta;
				for (int i = 1; i < m; i++)
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

		public bool IsTrinomial()
		{
			return k2 == 0 && k3 == 0;
		}
	}
}
