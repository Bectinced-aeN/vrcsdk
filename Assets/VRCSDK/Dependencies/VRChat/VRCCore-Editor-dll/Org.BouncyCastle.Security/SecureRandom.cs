using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Utilities;
using System;
using System.Threading;

namespace Org.BouncyCastle.Security
{
	internal class SecureRandom : Random
	{
		private static long counter = Times.NanoTime();

		private static readonly SecureRandom master = new SecureRandom(new CryptoApiRandomGenerator());

		protected readonly IRandomGenerator generator;

		private static readonly double DoubleScale = System.Math.Pow(2.0, 64.0);

		private static SecureRandom Master => master;

		public SecureRandom()
			: this(CreatePrng("SHA256", autoSeed: true))
		{
		}

		[Obsolete("Use GetInstance/SetSeed instead")]
		public SecureRandom(byte[] seed)
			: this(CreatePrng("SHA1", autoSeed: false))
		{
			SetSeed(seed);
		}

		public SecureRandom(IRandomGenerator generator)
			: base(0)
		{
			this.generator = generator;
		}

		private static long NextCounterValue()
		{
			return Interlocked.Increment(ref counter);
		}

		private static DigestRandomGenerator CreatePrng(string digestName, bool autoSeed)
		{
			IDigest digest = DigestUtilities.GetDigest(digestName);
			if (digest == null)
			{
				return null;
			}
			DigestRandomGenerator digestRandomGenerator = new DigestRandomGenerator(digest);
			if (autoSeed)
			{
				digestRandomGenerator.AddSeedMaterial(NextCounterValue());
				digestRandomGenerator.AddSeedMaterial(GetSeed(digest.GetDigestSize()));
			}
			return digestRandomGenerator;
		}

		public static SecureRandom GetInstance(string algorithm)
		{
			return GetInstance(algorithm, autoSeed: true);
		}

		public static SecureRandom GetInstance(string algorithm, bool autoSeed)
		{
			string text = Platform.ToUpperInvariant(algorithm);
			if (text.EndsWith("PRNG"))
			{
				string digestName = text.Substring(0, text.Length - "PRNG".Length);
				DigestRandomGenerator digestRandomGenerator = CreatePrng(digestName, autoSeed);
				if (digestRandomGenerator != null)
				{
					return new SecureRandom(digestRandomGenerator);
				}
			}
			throw new ArgumentException("Unrecognised PRNG algorithm: " + algorithm, "algorithm");
		}

		public static byte[] GetSeed(int length)
		{
			return Master.GenerateSeed(length);
		}

		public virtual byte[] GenerateSeed(int length)
		{
			SetSeed(DateTime.Now.Ticks);
			byte[] array = new byte[length];
			NextBytes(array);
			return array;
		}

		public virtual void SetSeed(byte[] seed)
		{
			generator.AddSeedMaterial(seed);
		}

		public virtual void SetSeed(long seed)
		{
			generator.AddSeedMaterial(seed);
		}

		public override int Next()
		{
			int num;
			do
			{
				num = (NextInt() & 0x7FFFFFFF);
			}
			while (num == 2147483647);
			return num;
		}

		public override int Next(int maxValue)
		{
			if (maxValue < 2)
			{
				if (maxValue < 0)
				{
					throw new ArgumentOutOfRangeException("maxValue", "cannot be negative");
				}
				return 0;
			}
			if ((maxValue & -maxValue) == maxValue)
			{
				int num = NextInt() & 0x7FFFFFFF;
				long num2 = (long)maxValue * (long)num >> 31;
				return (int)num2;
			}
			int num3;
			int num4;
			do
			{
				num3 = (NextInt() & 0x7FFFFFFF);
				num4 = num3 % maxValue;
			}
			while (num3 - num4 + (maxValue - 1) < 0);
			return num4;
		}

		public override int Next(int minValue, int maxValue)
		{
			if (maxValue <= minValue)
			{
				if (maxValue == minValue)
				{
					return minValue;
				}
				throw new ArgumentException("maxValue cannot be less than minValue");
			}
			int num = maxValue - minValue;
			if (num > 0)
			{
				return minValue + Next(num);
			}
			int num2;
			do
			{
				num2 = NextInt();
			}
			while (num2 < minValue || num2 >= maxValue);
			return num2;
		}

		public override void NextBytes(byte[] buf)
		{
			generator.NextBytes(buf);
		}

		public virtual void NextBytes(byte[] buf, int off, int len)
		{
			generator.NextBytes(buf, off, len);
		}

		public override double NextDouble()
		{
			return Convert.ToDouble((ulong)NextLong()) / DoubleScale;
		}

		public virtual int NextInt()
		{
			byte[] array = new byte[4];
			NextBytes(array);
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num = (num << 8) + (array[i] & 0xFF);
			}
			return num;
		}

		public virtual long NextLong()
		{
			return (long)(((ulong)(uint)NextInt() << 32) | (uint)NextInt());
		}
	}
}
