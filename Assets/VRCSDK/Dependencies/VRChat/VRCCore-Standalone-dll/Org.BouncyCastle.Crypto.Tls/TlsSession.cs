namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface TlsSession
	{
		byte[] SessionID
		{
			get;
		}

		bool IsResumable
		{
			get;
		}

		SessionParameters ExportSessionParameters();

		void Invalidate();
	}
}
