using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal class DerSet : Asn1Set
	{
		public static readonly DerSet Empty = new DerSet();

		public DerSet()
			: base(0)
		{
		}

		public DerSet(Asn1Encodable obj)
			: base(1)
		{
			AddObject(obj);
		}

		public DerSet(params Asn1Encodable[] v)
			: base(v.Length)
		{
			foreach (Asn1Encodable obj in v)
			{
				AddObject(obj);
			}
			Sort();
		}

		public DerSet(Asn1EncodableVector v)
			: this(v, needsSorting: true)
		{
		}

		internal DerSet(Asn1EncodableVector v, bool needsSorting)
			: base(v.Count)
		{
			foreach (Asn1Encodable item in v)
			{
				AddObject(item);
			}
			if (needsSorting)
			{
				Sort();
			}
		}

		public static DerSet FromVector(Asn1EncodableVector v)
		{
			return (v.Count >= 1) ? new DerSet(v) : Empty;
		}

		internal static DerSet FromVector(Asn1EncodableVector v, bool needsSorting)
		{
			return (v.Count >= 1) ? new DerSet(v, needsSorting) : Empty;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Asn1Encodable obj = (Asn1Encodable)enumerator.Current;
					derOutputStream.WriteObject(obj);
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
			derOutputStream.Dispose();
			byte[] bytes = memoryStream.ToArray();
			derOut.WriteEncoded(49, bytes);
		}
	}
}
