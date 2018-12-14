using System;

namespace Blake2Sharp
{
	internal class Blake2BHasher : Hasher
	{
		private readonly Blake2BCore core = new Blake2BCore();

		private readonly ulong[] rawConfig;

		private readonly byte[] key;

		private readonly int outputSizeInBytes;

		private static readonly Blake2BConfig DefaultConfig = new Blake2BConfig();

		public override void Init()
		{
			core.Initialize(rawConfig);
			if (key != null)
			{
				core.HashCore(key, 0, key.Length);
			}
		}

		public override byte[] Finish()
		{
			byte[] array = core.HashFinal();
			if (outputSizeInBytes != array.Length)
			{
				byte[] array2 = new byte[outputSizeInBytes];
				Array.Copy(array, array2, array2.Length);
				return array2;
			}
			return array;
		}

		public Blake2BHasher(Blake2BConfig config)
		{
			if (config == null)
			{
				config = DefaultConfig;
			}
			rawConfig = Blake2IvBuilder.ConfigB(config, null);
			if (config.Key != null && config.Key.Length != 0)
			{
				key = new byte[128];
				Array.Copy(config.Key, key, config.Key.Length);
			}
			outputSizeInBytes = config.OutputSizeInBytes;
			Init();
		}

		public override void Update(byte[] data, int start, int count)
		{
			core.HashCore(data, start, count);
		}
	}
}
