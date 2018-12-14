namespace Org.BouncyCastle.Crypto.Engines
{
	internal class CamelliaWrapEngine : Rfc3394WrapEngine
	{
		public CamelliaWrapEngine()
			: base(new CamelliaEngine())
		{
		}
	}
}
