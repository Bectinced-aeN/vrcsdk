using System;

namespace Org.BouncyCastle.Asn1.X509
{
	internal class X509Extension
	{
		internal bool critical;

		internal Asn1OctetString value;

		public bool IsCritical => critical;

		public Asn1OctetString Value => value;

		public X509Extension(DerBoolean critical, Asn1OctetString value)
		{
			if (critical == null)
			{
				throw new ArgumentNullException("critical");
			}
			this.critical = critical.IsTrue;
			this.value = value;
		}

		public X509Extension(bool critical, Asn1OctetString value)
		{
			this.critical = critical;
			this.value = value;
		}

		public Asn1Encodable GetParsedValue()
		{
			return ConvertValueToObject(this);
		}

		public override int GetHashCode()
		{
			int hashCode = Value.GetHashCode();
			return (!IsCritical) ? (~hashCode) : hashCode;
		}

		public override bool Equals(object obj)
		{
			X509Extension x509Extension = obj as X509Extension;
			if (x509Extension == null)
			{
				return false;
			}
			return Value.Equals(x509Extension.Value) && IsCritical == x509Extension.IsCritical;
		}

		public static Asn1Object ConvertValueToObject(X509Extension ext)
		{
			try
			{
				return Asn1Object.FromByteArray(ext.Value.GetOctets());
				IL_0016:
				Asn1Object result;
				return result;
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("can't convert extension", innerException);
				IL_0028:
				Asn1Object result;
				return result;
			}
		}
	}
}
