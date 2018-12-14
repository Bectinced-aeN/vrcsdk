namespace Org.BouncyCastle.Asn1.Iana
{
	internal abstract class IanaObjectIdentifiers
	{
		public static readonly DerObjectIdentifier IsakmpOakley = new DerObjectIdentifier("1.3.6.1.5.5.8.1");

		public static readonly DerObjectIdentifier HmacMD5 = new DerObjectIdentifier(IsakmpOakley + ".1");

		public static readonly DerObjectIdentifier HmacSha1 = new DerObjectIdentifier(IsakmpOakley + ".2");

		public static readonly DerObjectIdentifier HmacTiger = new DerObjectIdentifier(IsakmpOakley + ".3");

		public static readonly DerObjectIdentifier HmacRipeMD160 = new DerObjectIdentifier(IsakmpOakley + ".4");
	}
}
