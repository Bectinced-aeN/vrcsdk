using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto
{
	internal interface IBasicAgreement
	{
		void Init(ICipherParameters parameters);

		int GetFieldSize();

		BigInteger CalculateAgreement(ICipherParameters pubKey);
	}
}
