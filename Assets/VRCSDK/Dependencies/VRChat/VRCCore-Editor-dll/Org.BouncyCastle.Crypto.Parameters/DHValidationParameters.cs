using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class DHValidationParameters
	{
		private readonly byte[] seed;

		private readonly int counter;

		public int Counter => counter;

		public DHValidationParameters(byte[] seed, int counter)
		{
			if (seed == null)
			{
				throw new ArgumentNullException("seed");
			}
			this.seed = (byte[])seed.Clone();
			this.counter = counter;
		}

		public byte[] GetSeed()
		{
			return (byte[])seed.Clone();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DHValidationParameters dHValidationParameters = obj as DHValidationParameters;
			if (dHValidationParameters == null)
			{
				return false;
			}
			return Equals(dHValidationParameters);
		}

		protected bool Equals(DHValidationParameters other)
		{
			return counter == other.counter && Arrays.AreEqual(seed, other.seed);
		}

		public override int GetHashCode()
		{
			return counter.GetHashCode() ^ Arrays.GetHashCode(seed);
		}
	}
}
