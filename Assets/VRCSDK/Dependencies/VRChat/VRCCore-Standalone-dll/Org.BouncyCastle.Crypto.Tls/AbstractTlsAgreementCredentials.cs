namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class AbstractTlsAgreementCredentials : AbstractTlsCredentials, TlsAgreementCredentials, TlsCredentials
	{
		public abstract byte[] GenerateAgreement(AsymmetricKeyParameter peerPublicKey);
	}
}
