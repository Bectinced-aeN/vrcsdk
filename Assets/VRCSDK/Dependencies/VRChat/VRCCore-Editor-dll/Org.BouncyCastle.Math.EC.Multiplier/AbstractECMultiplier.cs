namespace Org.BouncyCastle.Math.EC.Multiplier
{
	internal abstract class AbstractECMultiplier : ECMultiplier
	{
		public virtual ECPoint Multiply(ECPoint p, BigInteger k)
		{
			int signValue = k.SignValue;
			if (signValue == 0 || p.IsInfinity)
			{
				return p.Curve.Infinity;
			}
			ECPoint eCPoint = MultiplyPositive(p, k.Abs());
			ECPoint p2 = (signValue <= 0) ? eCPoint.Negate() : eCPoint;
			return ECAlgorithms.ValidatePoint(p2);
		}

		protected abstract ECPoint MultiplyPositive(ECPoint p, BigInteger k);
	}
}
