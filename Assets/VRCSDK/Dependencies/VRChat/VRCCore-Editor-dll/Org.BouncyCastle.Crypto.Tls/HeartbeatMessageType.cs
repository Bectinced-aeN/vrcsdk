namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class HeartbeatMessageType
	{
		public const byte heartbeat_request = 1;

		public const byte heartbeat_response = 2;

		public static bool IsValid(byte heartbeatMessageType)
		{
			return heartbeatMessageType >= 1 && heartbeatMessageType <= 2;
		}
	}
}
