using System.Security.Cryptography;

namespace Blake2Sharp
{
	public abstract class Hasher
	{
		internal class HashAlgorithmAdapter : HashAlgorithm
		{
			private readonly Hasher _hasher;

			protected override void HashCore(byte[] array, int ibStart, int cbSize)
			{
				_hasher.Update(array, ibStart, cbSize);
			}

			protected override byte[] HashFinal()
			{
				return _hasher.Finish();
			}

			public override void Initialize()
			{
				_hasher.Init();
			}

			public HashAlgorithmAdapter(Hasher hasher)
			{
				_hasher = hasher;
			}
		}

		public abstract void Init();

		public abstract byte[] Finish();

		public abstract void Update(byte[] data, int start, int count);

		public void Update(byte[] data)
		{
			Update(data, 0, data.Length);
		}

		public HashAlgorithm AsHashAlgorithm()
		{
			return new HashAlgorithmAdapter(this);
		}
	}
}
