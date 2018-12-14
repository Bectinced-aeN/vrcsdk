using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Paddings
{
	internal interface IBlockCipherPadding
	{
		string PaddingName
		{
			get;
		}

		void Init(SecureRandom random);

		int AddPadding(byte[] input, int inOff);

		int PadCount(byte[] input);
	}
}
