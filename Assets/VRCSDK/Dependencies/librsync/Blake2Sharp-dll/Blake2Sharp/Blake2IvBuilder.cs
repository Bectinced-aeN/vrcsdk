using System;

namespace Blake2Sharp
{
	internal static class Blake2IvBuilder
	{
		private static readonly Blake2BTreeConfig SequentialTreeConfig = new Blake2BTreeConfig
		{
			IntermediateHashSize = 0,
			LeafSize = 0L,
			FanOut = 1,
			MaxHeight = 1
		};

		public static ulong[] ConfigB(Blake2BConfig config, Blake2BTreeConfig treeConfig)
		{
			bool flag = treeConfig == null;
			if (flag)
			{
				treeConfig = SequentialTreeConfig;
			}
			ulong[] array = new ulong[8];
			ulong[] array2 = new ulong[8];
			if ((config.OutputSizeInBytes <= 0) | (config.OutputSizeInBytes > 64))
			{
				throw new ArgumentOutOfRangeException("config.OutputSize");
			}
			ref ulong reference = ref array[0];
			reference |= (uint)config.OutputSizeInBytes;
			if (config.Key != null)
			{
				if (config.Key.Length > 64)
				{
					throw new ArgumentException("config.Key", "Key too long");
				}
				reference = ref array[0];
				reference |= (uint)(config.Key.Length << 8);
			}
			reference = ref array[0];
			reference |= (uint)(treeConfig.FanOut << 16);
			reference = ref array[0];
			reference |= (uint)(treeConfig.MaxHeight << 24);
			reference = ref array[0];
			reference |= (ulong)(uint)treeConfig.LeafSize << 32;
			if (!flag && (treeConfig.IntermediateHashSize <= 0 || treeConfig.IntermediateHashSize > 64))
			{
				throw new ArgumentOutOfRangeException("treeConfig.TreeIntermediateHashSize");
			}
			reference = ref array[2];
			reference |= (uint)(treeConfig.IntermediateHashSize << 8);
			if (config.Salt != null)
			{
				if (config.Salt.Length != 16)
				{
					throw new ArgumentException("config.Salt has invalid length");
				}
				array[4] = Blake2BCore.BytesToUInt64(config.Salt, 0);
				array[5] = Blake2BCore.BytesToUInt64(config.Salt, 8);
			}
			if (config.Personalization != null)
			{
				if (config.Personalization.Length != 16)
				{
					throw new ArgumentException("config.Personalization has invalid length");
				}
				array[6] = Blake2BCore.BytesToUInt64(config.Personalization, 0);
				array[6] = Blake2BCore.BytesToUInt64(config.Personalization, 8);
			}
			return array;
		}

		public static void ConfigBSetNode(ulong[] rawConfig, byte depth, ulong nodeOffset)
		{
			rawConfig[1] = nodeOffset;
			rawConfig[2] = (ulong)(((long)rawConfig[2] & -256L) | depth);
		}
	}
}
