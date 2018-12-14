using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	internal abstract class Asn1Set : Asn1Object, IEnumerable
	{
		private class Asn1SetParserImpl : Asn1SetParser, IAsn1Convertible
		{
			private readonly Asn1Set outer;

			private readonly int max;

			private int index;

			public Asn1SetParserImpl(Asn1Set outer)
			{
				this.outer = outer;
				max = outer.Count;
			}

			public IAsn1Convertible ReadObject()
			{
				if (index == max)
				{
					return null;
				}
				Asn1Encodable asn1Encodable = outer[index++];
				if (asn1Encodable is Asn1Sequence)
				{
					return ((Asn1Sequence)asn1Encodable).Parser;
				}
				if (asn1Encodable is Asn1Set)
				{
					return ((Asn1Set)asn1Encodable).Parser;
				}
				return asn1Encodable;
			}

			public virtual Asn1Object ToAsn1Object()
			{
				return outer;
			}
		}

		private readonly IList _set;

		public virtual Asn1Encodable this[int index]
		{
			get
			{
				return (Asn1Encodable)_set[index];
			}
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return Count;
			}
		}

		public virtual int Count => _set.Count;

		public Asn1SetParser Parser => new Asn1SetParserImpl(this);

		protected internal Asn1Set(int capacity)
		{
			_set = Platform.CreateArrayList(capacity);
		}

		public static Asn1Set GetInstance(object obj)
		{
			if (obj == null || obj is Asn1Set)
			{
				return (Asn1Set)obj;
			}
			if (obj is Asn1SetParser)
			{
				return GetInstance(((Asn1SetParser)obj).ToAsn1Object());
			}
			if (obj is byte[])
			{
				try
				{
					return GetInstance(Asn1Object.FromByteArray((byte[])obj));
					IL_0055:;
				}
				catch (IOException ex)
				{
					throw new ArgumentException("failed to construct set from byte[]: " + ex.Message);
					IL_0071:;
				}
			}
			if (obj is Asn1Encodable)
			{
				Asn1Object asn1Object = ((Asn1Encodable)obj).ToAsn1Object();
				if (asn1Object is Asn1Set)
				{
					return (Asn1Set)asn1Object;
				}
			}
			throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		public static Asn1Set GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			Asn1Object @object = obj.GetObject();
			if (explicitly)
			{
				if (!obj.IsExplicit())
				{
					throw new ArgumentException("object implicit - explicit expected.");
				}
				return (Asn1Set)@object;
			}
			if (obj.IsExplicit())
			{
				return new DerSet(@object);
			}
			if (@object is Asn1Set)
			{
				return (Asn1Set)@object;
			}
			if (@object is Asn1Sequence)
			{
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
				Asn1Sequence asn1Sequence = (Asn1Sequence)@object;
				foreach (Asn1Encodable item in asn1Sequence)
				{
					asn1EncodableVector.Add(item);
				}
				return new DerSet(asn1EncodableVector, needsSorting: false);
			}
			throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		public virtual IEnumerator GetEnumerator()
		{
			return _set.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return GetEnumerator();
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable GetObjectAt(int index)
		{
			return this[index];
		}

		public virtual Asn1Encodable[] ToArray()
		{
			Asn1Encodable[] array = new Asn1Encodable[Count];
			for (int i = 0; i < Count; i++)
			{
				array[i] = this[i];
			}
			return array;
		}

		protected override int Asn1GetHashCode()
		{
			int num = Count;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					num *= 17;
					num = ((current != null) ? (num ^ current.GetHashCode()) : (num ^ DerNull.Instance.GetHashCode()));
				}
				return num;
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

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			Asn1Set asn1Set = asn1Object as Asn1Set;
			if (asn1Set == null)
			{
				return false;
			}
			if (Count != asn1Set.Count)
			{
				return false;
			}
			IEnumerator enumerator = GetEnumerator();
			IEnumerator enumerator2 = asn1Set.GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				Asn1Object asn1Object2 = GetCurrent(enumerator).ToAsn1Object();
				Asn1Object obj = GetCurrent(enumerator2).ToAsn1Object();
				if (!asn1Object2.Equals(obj))
				{
					return false;
				}
			}
			return true;
		}

		private Asn1Encodable GetCurrent(IEnumerator e)
		{
			Asn1Encodable asn1Encodable = (Asn1Encodable)e.Current;
			if (asn1Encodable == null)
			{
				return DerNull.Instance;
			}
			return asn1Encodable;
		}

		private bool LessThanOrEqual(byte[] a, byte[] b)
		{
			int num = System.Math.Min(a.Length, b.Length);
			for (int i = 0; i != num; i++)
			{
				if (a[i] != b[i])
				{
					return a[i] < b[i];
				}
			}
			return num == a.Length;
		}

		protected internal void Sort()
		{
			if (_set.Count > 1)
			{
				bool flag = true;
				int num = _set.Count - 1;
				while (flag)
				{
					int i = 0;
					int num2 = 0;
					byte[] a = ((Asn1Encodable)_set[0]).GetEncoded();
					flag = false;
					for (; i != num; i++)
					{
						byte[] encoded = ((Asn1Encodable)_set[i + 1]).GetEncoded();
						if (LessThanOrEqual(a, encoded))
						{
							a = encoded;
						}
						else
						{
							object value = _set[i];
							_set[i] = _set[i + 1];
							_set[i + 1] = value;
							flag = true;
							num2 = i;
						}
					}
					num = num2;
				}
			}
		}

		protected internal void AddObject(Asn1Encodable obj)
		{
			_set.Add(obj);
		}

		public override string ToString()
		{
			return CollectionUtilities.ToString(_set);
		}
	}
}
