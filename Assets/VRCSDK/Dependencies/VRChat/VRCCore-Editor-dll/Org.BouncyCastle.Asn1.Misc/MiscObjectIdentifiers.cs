namespace Org.BouncyCastle.Asn1.Misc
{
	internal abstract class MiscObjectIdentifiers
	{
		public static readonly DerObjectIdentifier Netscape = new DerObjectIdentifier("2.16.840.1.113730.1");

		public static readonly DerObjectIdentifier NetscapeCertType = Netscape.Branch("1");

		public static readonly DerObjectIdentifier NetscapeBaseUrl = Netscape.Branch("2");

		public static readonly DerObjectIdentifier NetscapeRevocationUrl = Netscape.Branch("3");

		public static readonly DerObjectIdentifier NetscapeCARevocationUrl = Netscape.Branch("4");

		public static readonly DerObjectIdentifier NetscapeRenewalUrl = Netscape.Branch("7");

		public static readonly DerObjectIdentifier NetscapeCAPolicyUrl = Netscape.Branch("8");

		public static readonly DerObjectIdentifier NetscapeSslServerName = Netscape.Branch("12");

		public static readonly DerObjectIdentifier NetscapeCertComment = Netscape.Branch("13");

		public static readonly DerObjectIdentifier Verisign = new DerObjectIdentifier("2.16.840.1.113733.1");

		public static readonly DerObjectIdentifier VerisignCzagExtension = Verisign.Branch("6.3");

		public static readonly DerObjectIdentifier VerisignPrivate_6_9 = Verisign.Branch("6.9");

		public static readonly DerObjectIdentifier VerisignOnSiteJurisdictionHash = Verisign.Branch("6.11");

		public static readonly DerObjectIdentifier VerisignBitString_6_13 = Verisign.Branch("6.13");

		public static readonly DerObjectIdentifier VerisignDnbDunsNumber = Verisign.Branch("6.15");

		public static readonly DerObjectIdentifier VerisignIssStrongCrypto = Verisign.Branch("8.1");

		public static readonly string Novell = "2.16.840.1.113719";

		public static readonly DerObjectIdentifier NovellSecurityAttribs = new DerObjectIdentifier(Novell + ".1.9.4.1");

		public static readonly string Entrust = "1.2.840.113533.7";

		public static readonly DerObjectIdentifier EntrustVersionExtension = new DerObjectIdentifier(Entrust + ".65.0");
	}
}
