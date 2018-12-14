using System.Collections.Generic;

namespace Amazon.Runtime.Internal.Transform
{
	public static class UnmarshallerExtensions
	{
		public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> item)
		{
			dict.Add(item.Key, item.Value);
		}
	}
}
