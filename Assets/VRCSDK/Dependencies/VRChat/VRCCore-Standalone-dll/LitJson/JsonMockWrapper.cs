using System;
using System.Collections;

namespace LitJson
{
	internal class JsonMockWrapper : IEnumerable, IDictionary, IList, ICollection, IOrderedDictionary, IJsonWrapper
	{
		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		int ICollection.Count
		{
			get
			{
				return 0;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return null;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return null;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		object IOrderedDictionary.this[int idx]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public bool IsArray => false;

		public bool IsBoolean => false;

		public bool IsDouble => false;

		public bool IsInt => false;

		public bool IsLong => false;

		public bool IsObject => false;

		public bool IsString => false;

		int IList.Add(object value)
		{
			return 0;
		}

		void IList.Clear()
		{
		}

		bool IList.Contains(object value)
		{
			return false;
		}

		int IList.IndexOf(object value)
		{
			return -1;
		}

		void IList.Insert(int i, object v)
		{
		}

		void IList.Remove(object value)
		{
		}

		void IList.RemoveAt(int index)
		{
		}

		void ICollection.CopyTo(Array array, int index)
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return null;
		}

		void IDictionary.Add(object k, object v)
		{
		}

		void IDictionary.Clear()
		{
		}

		bool IDictionary.Contains(object key)
		{
			return false;
		}

		void IDictionary.Remove(object key)
		{
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return null;
		}

		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			return null;
		}

		void IOrderedDictionary.Insert(int i, object k, object v)
		{
		}

		void IOrderedDictionary.RemoveAt(int i)
		{
		}

		public bool GetBoolean()
		{
			return false;
		}

		public double GetDouble()
		{
			return 0.0;
		}

		public int GetInt()
		{
			return 0;
		}

		public JsonType GetJsonType()
		{
			return JsonType.None;
		}

		public long GetLong()
		{
			return 0L;
		}

		public string GetString()
		{
			return string.Empty;
		}

		public void SetBoolean(bool val)
		{
		}

		public void SetDouble(double val)
		{
		}

		public void SetInt(int val)
		{
		}

		public void SetJsonType(JsonType type)
		{
		}

		public void SetLong(long val)
		{
		}

		public void SetString(string val)
		{
		}

		public string ToJson()
		{
			return string.Empty;
		}

		public void ToJson(JsonWriter writer)
		{
		}
	}
}
