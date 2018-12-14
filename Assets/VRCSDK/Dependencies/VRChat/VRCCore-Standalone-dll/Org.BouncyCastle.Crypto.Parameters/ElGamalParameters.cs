using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class ElGamalParameters : ICipherParameters
	{
		private readonly BigInteger p;

		private readonly BigInteger g;

		private readonly int l;

		public BigInteger P => p;

		public BigInteger G => g;

		public int L => l;

		public ElGamalParameters(BigInteger p, BigInteger g)
			: this(p, g, 0)
		{
		}

		public ElGamalParameters(BigInteger p, BigInteger g, int l)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			this.p = p;
			this.g = g;
			this.l = l;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ElGamalParameters elGamalParameters = obj as ElGamalParameters;
			if (elGamalParameters == null)
			{
				return false;
			}
			return Equals(elGamalParameters);
		}

		protected bool Equals(ElGamalParameters other)
		{
			return p.Equals(other.p) && g.Equals(other.g) && l == other.l;
		}

		public override int GetHashCode()
		{
			return p.GetHashCode() ^ g.GetHashCode() ^ l;
		}
	}
}
