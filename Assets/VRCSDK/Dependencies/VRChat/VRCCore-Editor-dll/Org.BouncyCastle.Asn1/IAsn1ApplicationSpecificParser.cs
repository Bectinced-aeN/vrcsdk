namespace Org.BouncyCastle.Asn1
{
	internal interface IAsn1ApplicationSpecificParser : IAsn1Convertible
	{
		IAsn1Convertible ReadObject();
	}
}
