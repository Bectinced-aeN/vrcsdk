using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1
{
	internal class BerSequence : DerSequence
	{
		public new static readonly BerSequence Empty = new BerSequence();

		public BerSequence()
		{
		}

		public BerSequence(Asn1Encodable obj)
			: base(obj)
		{
		}

		public BerSequence(params Asn1Encodable[] v)
			: base(v)
		{
		}

		public BerSequence(Asn1EncodableVector v)
			: base(v)
		{
		}

		public new static BerSequence FromVector(Asn1EncodableVector v)
		{
			return (v.Count >= 1) ? new BerSequence(v) : Empty;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteByte(48);
				derOut.WriteByte(128);
				{
					IEnumerator enumerator = GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Asn1Encodable obj = (Asn1Encodable)enumerator.Current;
							derOut.WriteObject(obj);
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
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
