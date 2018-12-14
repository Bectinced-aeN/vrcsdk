using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1
{
	internal class Asn1EncodableVector : IEnumerable
	{
		private IList v = Platform.CreateArrayList();

		public Asn1Encodable this[int index]
		{
			get
			{
				return (Asn1Encodable)v[index];
			}
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return v.Count;
			}
		}

		public int Count => v.Count;

		public Asn1EncodableVector(params Asn1Encodable[] v)
		{
			Add(v);
		}

		public static Asn1EncodableVector FromEnumerable(IEnumerable e)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			foreach (Asn1Encodable item in e)
			{
				asn1EncodableVector.Add(item);
			}
			return asn1EncodableVector;
		}

		public void Add(params Asn1Encodable[] objs)
		{
			foreach (Asn1Encodable value in objs)
			{
				v.Add(value);
			}
		}

		public void AddOptional(params Asn1Encodable[] objs)
		{
			if (objs != null)
			{
				foreach (Asn1Encodable asn1Encodable in objs)
				{
					if (asn1Encodable != null)
					{
						v.Add(asn1Encodable);
					}
				}
			}
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable Get(int index)
		{
			return this[index];
		}

		public IEnumerator GetEnumerator()
		{
			return v.GetEnumerator();
		}
	}
}