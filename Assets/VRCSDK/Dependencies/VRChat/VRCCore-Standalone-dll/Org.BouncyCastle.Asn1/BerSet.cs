using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1
{
	internal class BerSet : DerSet
	{
		public new static readonly BerSet Empty = new BerSet();

		public BerSet()
		{
		}

		public BerSet(Asn1Encodable obj)
			: base(obj)
		{
		}

		public BerSet(Asn1EncodableVector v)
			: base(v, needsSorting: false)
		{
		}

		internal BerSet(Asn1EncodableVector v, bool needsSorting)
			: base(v, needsSorting)
		{
		}

		public new static BerSet FromVector(Asn1EncodableVector v)
		{
			return (v.Count >= 1) ? new BerSet(v) : Empty;
		}

		internal new static BerSet FromVector(Asn1EncodableVector v, bool needsSorting)
		{
			return (v.Count >= 1) ? new BerSet(v, needsSorting) : Empty;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteByte(49);
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
