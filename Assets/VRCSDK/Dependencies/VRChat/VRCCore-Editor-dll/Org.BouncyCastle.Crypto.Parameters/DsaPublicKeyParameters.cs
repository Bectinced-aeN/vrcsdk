using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class DsaPublicKeyParameters : DsaKeyParameters
	{
		private readonly BigInteger y;

		public BigInteger Y => y;

		public DsaPublicKeyParameters(BigInteger y, DsaParameters parameters)
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
			DsaPublicKeyParameters dsaPublicKeyParameters = obj as DsaPublicKeyParameters;
			if (dsaPublicKeyParameters == null)
			{
				return false;
			}
			return Equals(dsaPublicKeyParameters);
		}

		protected bool Equals(DsaPublicKeyParameters other)
		{
			return y.Equals(other.y) && Equals((DsaKeyParameters)other);
		}

		public override int GetHashCode()
		{
			return y.GetHashCode() ^ base.GetHashCode();
		}
	}
}
