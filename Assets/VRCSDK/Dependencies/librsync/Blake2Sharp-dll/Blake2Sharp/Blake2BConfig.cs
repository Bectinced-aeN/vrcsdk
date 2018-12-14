using System;

namespace Blake2Sharp
{
	public sealed class Blake2BConfig : ICloneable
	{
		public byte[] Personalization
		{
			get;
			set;
		}

		public byte[] Salt
		{
			get;
			set;
		}

		public byte[] Key
		{
			get;
			set;
		}

		public int OutputSizeInBytes
		{
			get;
			set;
		}

		public int OutputSizeInBits
		{
			get
			{
				return OutputSizeInBytes * 8;
			}
			set
			{
				if (value % 8 == 0)
				{
					throw new ArgumentException("Output size must be a multiple of 8 bits");
				}
				OutputSizeInBytes = value / 8;
			}
		}

		public Blake2BConfig()
		{
			OutputSizeInBytes = 64;
		}

		public Blake2BConfig Clone()
		{
			Blake2BConfig blake2BConfig = new Blake2BConfig();
			blake2BConfig.OutputSizeInBytes = OutputSizeInBytes;
			if (Key != null)
			{
				blake2BConfig.Key = (byte[])Key.Clone();
			}
			if (Personalization != null)
			{
				blake2BConfig.Personalization = (byte[])Personalization.Clone();
			}
			if (Salt != null)
			{
				blake2BConfig.Salt = (byte[])Salt.Clone();
			}
			return blake2BConfig;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}
