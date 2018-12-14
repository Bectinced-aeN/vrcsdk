namespace Org.BouncyCastle.Asn1
{
	internal class BerApplicationSpecific : DerApplicationSpecific
	{
		public BerApplicationSpecific(int tagNo, Asn1EncodableVector vec)
			: base(tagNo, vec)
		{
		}
	}
}
