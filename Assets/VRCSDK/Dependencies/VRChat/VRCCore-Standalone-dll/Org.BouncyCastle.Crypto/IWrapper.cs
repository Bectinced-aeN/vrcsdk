namespace Org.BouncyCastle.Crypto
{
	internal interface IWrapper
	{
		string AlgorithmName
		{
			get;
		}

		void Init(bool forWrapping, ICipherParameters parameters);

		byte[] Wrap(byte[] input, int inOff, int length);

		byte[] Unwrap(byte[] input, int inOff, int length);
	}
}
