using System;

namespace Org.BouncyCastle.Asn1.X9
{
	internal class DHPublicKey : Asn1Encodable
	{
		private readonly DerInteger y;

		public DerInteger Y => y;

		public DHPublicKey(DerInteger y)
		{
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			this.y = y;
		}

		public static DHPublicKey GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(DerInteger.GetInstance(obj, isExplicit));
		}

		public static DHPublicKey GetInstance(object obj)
		{
			if (obj == null || obj is DHPublicKey)
			{
				return (DHPublicKey)obj;
			}
			if (obj is DerInteger)
			{
				return new DHPublicKey((DerInteger)obj);
			}
			throw new ArgumentException("Invalid DHPublicKey: " + obj.GetType().FullName, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			return y;
		}
	}
}
