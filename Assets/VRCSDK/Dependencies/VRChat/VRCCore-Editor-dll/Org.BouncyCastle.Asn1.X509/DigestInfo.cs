using System;

namespace Org.BouncyCastle.Asn1.X509
{
	internal class DigestInfo : Asn1Encodable
	{
		private readonly byte[] digest;

		private readonly AlgorithmIdentifier algID;

		public AlgorithmIdentifier AlgorithmID => algID;

		public DigestInfo(AlgorithmIdentifier algID, byte[] digest)
		{
			this.digest = digest;
			this.algID = algID;
		}

		private DigestInfo(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			algID = AlgorithmIdentifier.GetInstance(seq[0]);
			digest = Asn1OctetString.GetInstance(seq[1]).GetOctets();
		}

		public static DigestInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static DigestInfo GetInstance(object obj)
		{
			if (obj is DigestInfo)
			{
				return (DigestInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new DigestInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public byte[] GetDigest()
		{
			return digest;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(algID, new DerOctetString(digest));
		}
	}
}
