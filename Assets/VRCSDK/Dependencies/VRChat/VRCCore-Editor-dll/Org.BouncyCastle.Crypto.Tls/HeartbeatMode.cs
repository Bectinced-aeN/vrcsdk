namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class HeartbeatMode
	{
		public const byte peer_allowed_to_send = 1;

		public const byte peer_not_allowed_to_send = 2;

		public static bool IsValid(byte heartbeatMode)
		{
			return heartbeatMode >= 1 && heartbeatMode <= 2;
		}
	}
}
