namespace Org.BouncyCastle.Asn1
{
	internal interface Asn1TaggedObjectParser : IAsn1Convertible
	{
		int TagNo
		{
			get;
		}

		IAsn1Convertible GetObjectParser(int tag, bool isExplicit);
	}
}
