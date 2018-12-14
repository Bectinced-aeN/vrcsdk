namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	internal interface IGcmExponentiator
	{
		void Init(byte[] x);

		void ExponentiateX(long pow, byte[] output);
	}
}
