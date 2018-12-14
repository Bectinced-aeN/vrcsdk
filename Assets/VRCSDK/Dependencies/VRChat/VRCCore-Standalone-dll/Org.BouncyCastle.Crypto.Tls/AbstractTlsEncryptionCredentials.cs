namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class AbstractTlsEncryptionCredentials : AbstractTlsCredentials, TlsCredentials, TlsEncryptionCredentials
	{
		public abstract byte[] DecryptPreMasterSecret(byte[] encryptedPreMasterSecret);
	}
}
