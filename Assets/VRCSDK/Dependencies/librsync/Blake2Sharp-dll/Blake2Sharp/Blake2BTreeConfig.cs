using System;

namespace Blake2Sharp
{
	public sealed class Blake2BTreeConfig : ICloneable
	{
		public int IntermediateHashSize
		{
			get;
			set;
		}

		public int MaxHeight
		{
			get;
			set;
		}

		public long LeafSize
		{
			get;
			set;
		}

		public int FanOut
		{
			get;
			set;
		}

		public Blake2BTreeConfig()
		{
			IntermediateHashSize = 64;
		}

		public Blake2BTreeConfig Clone()
		{
			Blake2BTreeConfig blake2BTreeConfig = new Blake2BTreeConfig();
			blake2BTreeConfig.IntermediateHashSize = IntermediateHashSize;
			blake2BTreeConfig.MaxHeight = MaxHeight;
			blake2BTreeConfig.LeafSize = LeafSize;
			blake2BTreeConfig.FanOut = FanOut;
			return blake2BTreeConfig;
		}

		public static Blake2BTreeConfig CreateInterleaved(int parallelism)
		{
			Blake2BTreeConfig blake2BTreeConfig = new Blake2BTreeConfig();
			blake2BTreeConfig.FanOut = parallelism;
			blake2BTreeConfig.MaxHeight = 2;
			blake2BTreeConfig.IntermediateHashSize = 64;
			return blake2BTreeConfig;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}
