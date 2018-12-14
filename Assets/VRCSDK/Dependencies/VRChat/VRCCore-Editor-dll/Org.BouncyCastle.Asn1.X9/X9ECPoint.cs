using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Asn1.X9
{
	internal class X9ECPoint : Asn1Encodable
	{
		private readonly ECPoint p;

		public ECPoint Point => p;

		public X9ECPoint(ECPoint p)
		{
			this.p = p.Normalize();
		}

		public X9ECPoint(ECCurve c, Asn1OctetString s)
		{
			p = c.DecodePoint(s.GetOctets());
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerOctetString(p.GetEncoded());
		}
	}
}
