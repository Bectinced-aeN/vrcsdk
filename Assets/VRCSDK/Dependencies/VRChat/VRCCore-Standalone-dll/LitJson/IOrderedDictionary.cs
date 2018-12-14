using System.Collections;

namespace LitJson
{
	internal interface IOrderedDictionary : IEnumerable, ICollection, IDictionary
	{
		object this[int index]
		{
			get;
			set;
		}

		new IDictionaryEnumerator GetEnumerator();

		void Insert(int index, object key, object value);

		void RemoveAt(int index);
	}
}
