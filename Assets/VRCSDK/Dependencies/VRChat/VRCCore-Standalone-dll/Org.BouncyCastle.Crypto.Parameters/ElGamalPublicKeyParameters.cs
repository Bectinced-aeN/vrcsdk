using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class ElGamalPublicKeyParameters : ElGamalKeyParameters
	{
		private readonly BigInteger y;

		public BigInteger Y => y;

		public ElGamalPublicKeyParameters(BigInteger y, ElGamalParameters parameters)
			: base(isPrivate: false, parameters)
		{
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			this.y = y;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ElGamalPublicKeyParameters elGamalPublicKeyParameters = obj as ElGamalPublicKeyParameters;
			if (elGamalPublicKeyParameters == null)
			{
				return false;
			}
			return Equals(elGamalPublicKeyParameters);
		}

		protected bool Equals(ElGamalPublicKeyParameters other)
		{
			return y.Equals(other.y) && Equals((ElGamalKeyParameters)other);
		}

		public override int GetHashCode()
		{
			return y.GetHashCode() ^ base.GetHashCode();
		}
	}
}
