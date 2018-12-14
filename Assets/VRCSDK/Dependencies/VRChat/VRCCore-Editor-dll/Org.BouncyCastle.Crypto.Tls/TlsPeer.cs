using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal interface TlsPeer
	{
		bool ShouldUseGmtUnixTime();

		void NotifySecureRenegotiation(bool secureRenegotiation);

		TlsCompression GetCompression();

		TlsCipher GetCipher();

		void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause);

		void NotifyAlertReceived(byte alertLevel, byte alertDescription);

		void NotifyHandshakeComplete();
	}
}
