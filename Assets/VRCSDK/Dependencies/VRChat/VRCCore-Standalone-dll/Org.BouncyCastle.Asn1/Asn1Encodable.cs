using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal abstract class Asn1Encodable : IAsn1Convertible
	{
		public const string Der = "DER";

		public const string Ber = "BER";

		public byte[] GetEncoded()
		{
			MemoryStream memoryStream = new MemoryStream();
			Asn1OutputStream asn1OutputStream = new Asn1OutputStream(memoryStream);
			asn1OutputStream.WriteObject(this);
			return memoryStream.ToArray();
		}

		public byte[] GetEncoded(string encoding)
		{
			if (encoding.Equals("DER"))
			{
				MemoryStream memoryStream = new MemoryStream();
				DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
				derOutputStream.WriteObject(this);
				return memoryStream.ToArray();
			}
			return GetEncoded();
		}

		public byte[] GetDerEncoded()
		{
			try
			{
				return GetEncoded("DER");
				IL_0011:
				byte[] result;
				return result;
			}
			catch (IOException)
			{
				return null;
				IL_001e:
				byte[] result;
				return result;
			}
		}

		public sealed override int GetHashCode()
		{
			return ToAsn1Object().CallAsn1GetHashCode();
		}

		public sealed override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			IAsn1Convertible asn1Convertible = obj as IAsn1Convertible;
			if (asn1Convertible == null)
			{
				return false;
			}
			Asn1Object asn1Object = ToAsn1Object();
			Asn1Object asn1Object2 = asn1Convertible.ToAsn1Object();
			return asn1Object == asn1Object2 || asn1Object.CallAsn1Equals(asn1Object2);
		}

		public abstract Asn1Object ToAsn1Object();
	}
}
