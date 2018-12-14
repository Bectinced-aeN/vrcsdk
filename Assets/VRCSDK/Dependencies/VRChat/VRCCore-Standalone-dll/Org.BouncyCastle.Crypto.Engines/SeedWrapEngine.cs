namespace Org.BouncyCastle.Crypto.Engines
{
	internal class SeedWrapEngine : Rfc3394WrapEngine
	{
		public SeedWrapEngine()
			: base(new SeedEngine())
		{
		}
	}
}
