using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal abstract class Asn1OctetString : Asn1Object, Asn1OctetStringParser, IAsn1Convertible
	{
		internal byte[] str;

		public Asn1OctetStringParser Parser => this;

		internal Asn1OctetString(byte[] str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
		}

		internal Asn1OctetString(Asn1Encodable obj)
		{
			try
			{
				str = obj.GetEncoded("DER");
			}
			catch (IOException ex)
			{
				throw new ArgumentException("Error processing object : " + ex.ToString());
				IL_0033:;
			}
		}

		public static Asn1OctetString GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is Asn1OctetString)
			{
				return GetInstance(@object);
			}
			return BerOctetString.FromSequence(Asn1Sequence.GetInstance(@object));
		}

		public static Asn1OctetString GetInstance(object obj)
		{
			if (obj == null || obj is Asn1OctetString)
			{
				return (Asn1OctetString)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public Stream GetOctetStream()
		{
			return new MemoryStream(str, writable: false);
		}

		public virtual byte[] GetOctets()
		{
			return str;
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerOctetString derOctetString = asn1Object as DerOctetString;
			if (derOctetString == null)
			{
				return false;
			}
			return Arrays.AreEqual(GetOctets(), derOctetString.GetOctets());
		}

		public override string ToString()
		{
			return "#" + Hex.ToHexString(str);
		}
	}
}
