namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface TlsEncryptionCredentials : TlsCredentials
	{
		byte[] DecryptPreMasterSecret(byte[] encryptedPreMasterSecret);
	}
}
