using System;

namespace Org.BouncyCastle.Asn1.X509
{
	internal class AlgorithmIdentifier : Asn1Encodable
	{
		private readonly DerObjectIdentifier objectID;

		private readonly Asn1Encodable parameters;

		private readonly bool parametersDefined;

		public virtual DerObjectIdentifier ObjectID => objectID;

		public Asn1Encodable Parameters => parameters;

		public AlgorithmIdentifier(DerObjectIdentifier objectID)
		{
			this.objectID = objectID;
		}

		public AlgorithmIdentifier(string objectID)
		{
			this.objectID = new DerObjectIdentifier(objectID);
		}

		public AlgorithmIdentifier(DerObjectIdentifier objectID, Asn1Encodable parameters)
		{
			this.objectID = objectID;
			this.parameters = parameters;
			parametersDefined = true;
		}

		internal AlgorithmIdentifier(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			objectID = DerObjectIdentifier.GetInstance(seq[0]);
			parametersDefined = (seq.Count == 2);
			if (parametersDefined)
			{
				parameters = seq[1];
			}
		}

		public static AlgorithmIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static AlgorithmIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is AlgorithmIdentifier)
			{
				return (AlgorithmIdentifier)obj;
			}
			if (obj is DerObjectIdentifier)
			{
				return new AlgorithmIdentifier((DerObjectIdentifier)obj);
			}
			if (obj is string)
			{
				return new AlgorithmIdentifier((string)obj);
			}
			return new AlgorithmIdentifier(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(objectID);
			if (parametersDefined)
			{
				if (parameters != null)
				{
					asn1EncodableVector.Add(parameters);
				}
				else
				{
					asn1EncodableVector.Add(DerNull.Instance);
				}
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
