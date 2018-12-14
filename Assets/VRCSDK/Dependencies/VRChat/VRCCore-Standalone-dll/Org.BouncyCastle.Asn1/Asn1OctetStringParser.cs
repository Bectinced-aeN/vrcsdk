using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal interface Asn1OctetStringParser : IAsn1Convertible
	{
		Stream GetOctetStream();
	}
}
