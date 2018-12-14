using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class ECPrivateKeyParameters : ECKeyParameters
	{
		private readonly BigInteger d;

		public BigInteger D => d;

		public ECPrivateKeyParameters(BigInteger d, ECDomainParameters parameters)
			: this("EC", d, parameters)
		{
		}

		[Obsolete("Use version with explicit 'algorithm' parameter")]
		public ECPrivateKeyParameters(BigInteger d, DerObjectIdentifier publicKeyParamSet)
			: base("ECGOST3410", isPrivate: true, publicKeyParamSet)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			this.d = d;
		}

		public ECPrivateKeyParameters(string algorithm, BigInteger d, ECDomainParameters parameters)
			: base(algorithm, isPrivate: true, parameters)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			this.d = d;
		}

		public ECPrivateKeyParameters(string algorithm, BigInteger d, DerObjectIdentifier publicKeyParamSet)
			: base(algorithm, isPrivate: true, publicKeyParamSet)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}
			this.d = d;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ECPrivateKeyParameters eCPrivateKeyParameters = obj as ECPrivateKeyParameters;
			if (eCPrivateKeyParameters == null)
			{
				return false;
			}
			return Equals(eCPrivateKeyParameters);
		}

		protected bool Equals(ECPrivateKeyParameters other)
		{
			return d.Equals(other.d) && Equals((ECKeyParameters)other);
		}

		public override int GetHashCode()
		{
			return d.GetHashCode() ^ base.GetHashCode();
		}
	}
}
