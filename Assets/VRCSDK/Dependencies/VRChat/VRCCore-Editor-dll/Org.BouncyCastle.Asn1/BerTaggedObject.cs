using Org.BouncyCastle.Utilities;
using System.Collections;

namespace Org.BouncyCastle.Asn1
{
	internal class BerTaggedObject : DerTaggedObject
	{
		public BerTaggedObject(int tagNo, Asn1Encodable obj)
			: base(tagNo, obj)
		{
		}

		public BerTaggedObject(bool explicitly, int tagNo, Asn1Encodable obj)
			: base(explicitly, tagNo, obj)
		{
		}

		public BerTaggedObject(int tagNo)
			: base(explicitly: false, tagNo, BerSequence.Empty)
		{
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteTag(160, tagNo);
				derOut.WriteByte(128);
				if (!IsEmpty())
				{
					if (!explicitly)
					{
						IEnumerable enumerable;
						if (base.obj is Asn1OctetString)
						{
							if (base.obj is BerOctetString)
							{
								enumerable = (BerOctetString)base.obj;
							}
							else
							{
								Asn1OctetString asn1OctetString = (Asn1OctetString)base.obj;
								enumerable = new BerOctetString(asn1OctetString.GetOctets());
							}
						}
						else if (base.obj is Asn1Sequence)
						{
							enumerable = (Asn1Sequence)base.obj;
						}
						else
						{
							if (!(base.obj is Asn1Set))
							{
								throw Platform.CreateNotImplementedException(base.obj.GetType().Name);
							}
							enumerable = (Asn1Set)base.obj;
						}
						foreach (Asn1Encodable item in enumerable)
						{
							derOut.WriteObject(item);
						}
					}
					else
					{
						derOut.WriteObject(obj);
					}
				}
				derOut.WriteByte(0);
				derOut.WriteByte(0);
			}
			else
			{
				base.Encode(derOut);
			}
		}
	}
}
