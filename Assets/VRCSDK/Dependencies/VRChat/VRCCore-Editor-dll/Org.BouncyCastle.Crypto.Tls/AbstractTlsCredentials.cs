namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class AbstractTlsCredentials : TlsCredentials
	{
		public abstract Certificate Certificate
		{
			get;
		}
	}
}
