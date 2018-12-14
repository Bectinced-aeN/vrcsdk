namespace Org.BouncyCastle.Crypto
{
	internal interface ISignerWithRecovery : ISigner
	{
		bool HasFullMessage();

		byte[] GetRecoveredMessage();

		void UpdateWithRecoveredMessage(byte[] signature);
	}
}
