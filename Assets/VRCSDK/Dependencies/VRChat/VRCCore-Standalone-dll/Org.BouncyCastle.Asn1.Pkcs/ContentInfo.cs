namespace Org.BouncyCastle.Asn1.Pkcs
{
	internal class ContentInfo : Asn1Encodable
	{
		private readonly DerObjectIdentifier contentType;

		private readonly Asn1Encodable content;

		public DerObjectIdentifier ContentType => contentType;

		public Asn1Encodable Content => content;

		private ContentInfo(Asn1Sequence seq)
		{
			contentType = (DerObjectIdentifier)seq[0];
			if (seq.Count > 1)
			{
				content = ((Asn1TaggedObject)seq[1]).GetObject();
			}
		}

		public ContentInfo(DerObjectIdentifier contentType, Asn1Encodable content)
		{
			this.contentType = contentType;
			this.content = content;
		}

		public static ContentInfo GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			ContentInfo contentInfo = obj as ContentInfo;
			if (contentInfo != null)
			{
				return contentInfo;
			}
			return new ContentInfo(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(contentType);
			if (content != null)
			{
				asn1EncodableVector.Add(new BerTaggedObject(0, content));
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
