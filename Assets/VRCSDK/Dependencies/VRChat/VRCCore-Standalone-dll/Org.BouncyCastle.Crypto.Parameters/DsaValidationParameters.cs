using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	internal class DsaValidationParameters
	{
		private readonly byte[] seed;

		private readonly int counter;

		private readonly int usageIndex;

		public virtual int Counter => counter;

		public virtual int UsageIndex => usageIndex;

		public DsaValidationParameters(byte[] seed, int counter)
			: this(seed, counter, -1)
		{
		}

		public DsaValidationParameters(byte[] seed, int counter, int usageIndex)
		{
			if (seed == null)
			{
				throw new ArgumentNullException("seed");
			}
			this.seed = (byte[])seed.Clone();
			this.counter = counter;
			this.usageIndex = usageIndex;
		}

		public virtual byte[] GetSeed()
		{
			return (byte[])seed.Clone();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DsaValidationParameters dsaValidationParameters = obj as DsaValidationParameters;
			if (dsaValidationParameters == null)
			{
				return false;
			}
			return Equals(dsaValidationParameters);
		}

		protected virtual bool Equals(DsaValidationParameters other)
		{
			return counter == other.counter && Arrays.AreEqual(seed, other.seed);
		}

		public override int GetHashCode()
		{
			return counter.GetHashCode() ^ Arrays.GetHashCode(seed);
		}
	}
}
