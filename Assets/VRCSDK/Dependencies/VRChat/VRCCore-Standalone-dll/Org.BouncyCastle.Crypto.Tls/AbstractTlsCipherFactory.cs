namespace Org.BouncyCastle.Crypto.Tls
{
	internal class AbstractTlsCipherFactory : TlsCipherFactory
	{
		public virtual TlsCipher CreateCipher(TlsContext context, int encryptionAlgorithm, int macAlgorithm)
		{
			throw new TlsFatalAlert(80);
		}
	}
}
