namespace Org.BouncyCastle.Crypto.Engines
{
	internal class AesWrapEngine : Rfc3394WrapEngine
	{
		public AesWrapEngine()
			: base(new AesEngine())
		{
		}
	}
}
