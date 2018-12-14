using System;
using System.Collections.Generic;

namespace Amazon.Runtime.Internal.Util
{
	public class LruCache<TKey, TValue> where TKey : class where TValue : class
	{
		private readonly object cacheLock = new object();

		private Dictionary<TKey, LruListItem<TKey, TValue>> cache;

		private LruList<TKey, TValue> lruList;

		public int MaxEntries
		{
			get;
			private set;
		}

		public int Count
		{
			get
			{
				lock (cacheLock)
				{
					return cache.Count;
				}
			}
		}

		public LruCache(int maxEntries)
		{
			if (maxEntries < 1)
			{
				throw new ArgumentException("maxEntries must be greater than zero.");
			}
			MaxEntries = maxEntries;
			cache = new Dictionary<TKey, LruListItem<TKey, TValue>>();
			lruList = new LruList<TKey, TValue>();
		}

		public void AddOrUpdate(TKey key, TValue value)
		{
			lock (cacheLock)
			{
				if (cache.TryGetValue(key, out LruListItem<TKey, TValue> value2))
				{
					value2.Value = value;
					lruList.Touch(value2);
				}
				else
				{
					LruListItem<TKey, TValue> lruListItem = new LruListItem<TKey, TValue>(key, value);
					while (cache.Count >= MaxEntries)
					{
						cache.Remove(lruList.EvictOldest());
					}
					lruList.Add(lruListItem);
					cache.Add(key, lruListItem);
				}
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			lock (cacheLock)
			{
				if (cache.TryGetValue(key, out LruListItem<TKey, TValue> value2))
				{
					lruList.Touch(value2);
					value = value2.Value;
					return true;
				}
				value = null;
				return false;
			}
		}

		public void Clear()
		{
			lock (cacheLock)
			{
				cache.Clear();
				lruList.Clear();
			}
		}
	}
}
