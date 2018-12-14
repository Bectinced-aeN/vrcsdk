using Org.BouncyCastle.Math.EC.Abc;
using Org.BouncyCastle.Math.Field;
using System;

namespace Org.BouncyCastle.Math.EC
{
	internal abstract class AbstractF2mCurve : ECCurve
	{
		private BigInteger[] si;

		public virtual bool IsKoblitz => m_order != null && m_cofactor != null && m_b.IsOne && (m_a.IsZero || m_a.IsOne);

		protected AbstractF2mCurve(int m, int k1, int k2, int k3)
			: base(BuildField(m, k1, k2, k3))
		{
		}

		public static BigInteger Inverse(int m, int[] ks, BigInteger x)
		{
			return new LongArray(x).ModInverse(m, ks).ToBigInteger();
		}

		private static IFiniteField BuildField(int m, int k1, int k2, int k3)
		{
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
				return FiniteFields.GetBinaryExtensionField(new int[3]
				{
					0,
					k1,
					m
				});
			}
			if (k2 <= k1)
			{
				throw new ArgumentException("k2 must be > k1");
			}
			if (k3 <= k2)
			{
				throw new ArgumentException("k3 must be > k2");
			}
			return FiniteFields.GetBinaryExtensionField(new int[5]
			{
				0,
				k1,
				k2,
				k3,
				m
			});
		}

		public override ECPoint CreatePoint(BigInteger x, BigInteger y, bool withCompression)
		{
			ECFieldElement eCFieldElement = FromBigInteger(x);
			ECFieldElement eCFieldElement2 = FromBigInteger(y);
			int coordinateSystem = CoordinateSystem;
			if (coordinateSystem == 5 || coordinateSystem == 6)
			{
				if (eCFieldElement.IsZero)
				{
					if (!eCFieldElement2.Square().Equals(B))
					{
						throw new ArgumentException();
					}
				}
				else
				{
					eCFieldElement2 = eCFieldElement2.Divide(eCFieldElement).Add(eCFieldElement);
				}
			}
			return CreateRawPoint(eCFieldElement, eCFieldElement2, withCompression);
		}

		internal virtual BigInteger[] GetSi()
		{
			if (si == null)
			{
				lock (this)
				{
					if (si == null)
					{
						si = Tnaf.GetSi(this);
					}
				}
			}
			return si;
		}
	}
}
