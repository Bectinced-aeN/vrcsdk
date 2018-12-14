using System;
using System.Collections.Generic;
using System.Linq;

namespace Amazon.Runtime.Internal.Util
{
	internal class Cache<TKey, TValue> : ICache<TKey, TValue>, ICache
	{
		private class CacheItem<T>
		{
			private T _value;

			public T Value
			{
				get
				{
					LastUseTime = DateTime.Now;
					return _value;
				}
				private set
				{
					_value = value;
				}
			}

			public DateTime LastUseTime
			{
				get;
				private set;
			}

			public CacheItem(T value)
			{
				Value = value;
				LastUseTime = DateTime.Now;
			}
		}

		private Dictionary<TKey, CacheItem<TValue>> Contents;

		private readonly object CacheLock = new object();

		public static TimeSpan DefaultMaximumItemLifespan = TimeSpan.FromHours(6.0);

		public static TimeSpan DefaultCacheClearPeriod = TimeSpan.FromHours(1.0);

		private TimeSpan maximumItemLifespan;

		private TimeSpan cacheClearPeriod;

		public DateTime LastCacheClean
		{
			get;
			private set;
		}

		public List<TKey> Keys => Contents.Keys.ToList();

		public TimeSpan MaximumItemLifespan
		{
			get
			{
				return maximumItemLifespan;
			}
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				maximumItemLifespan = value;
			}
		}

		public TimeSpan CacheClearPeriod
		{
			get
			{
				return cacheClearPeriod;
			}
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				cacheClearPeriod = value;
			}
		}

		public int ItemCount => Contents.Count;

		public Cache(IEqualityComparer<TKey> keyComparer = null)
		{
			Contents = new Dictionary<TKey, CacheItem<TValue>>(keyComparer);
			MaximumItemLifespan = DefaultMaximumItemLifespan;
			CacheClearPeriod = DefaultCacheClearPeriod;
		}

		public TValue GetValue(TKey key, Func<TKey, TValue> creator)
		{
			bool isStaleItem;
			return GetValueHelper(key, out isStaleItem, creator);
		}

		public TValue GetValue(TKey key, Func<TKey, TValue> creator, out bool isStaleItem)
		{
			return GetValueHelper(key, out isStaleItem, creator);
		}

		public void Clear(TKey key)
		{
			lock (CacheLock)
			{
				Contents.Remove(key);
			}
		}

		public void Clear()
		{
			lock (CacheLock)
			{
				Contents.Clear();
				LastCacheClean = DateTime.Now;
			}
		}

		public TOut UseCache<TOut>(TKey key, Func<TOut> operation, Action onError, Predicate<Exception> shouldRetryForException)
		{
			TOut val = default(TOut);
			try
			{
				return operation();
			}
			catch (Exception obj)
			{
				if (!(shouldRetryForException?.Invoke(obj) ?? true))
				{
					throw;
				}
				Clear(key);
				onError?.Invoke();
				return operation();
			}
		}

		private TValue GetValueHelper(TKey key, out bool isStaleItem, Func<TKey, TValue> creator = null)
		{
			isStaleItem = true;
			CacheItem<TValue> value = null;
			if (AWSConfigs.UseSdkCache)
			{
				if (!Contents.TryGetValue(key, out value) || !IsValidItem(value))
				{
					lock (CacheLock)
					{
						if (!Contents.TryGetValue(key, out value) || !IsValidItem(value))
						{
							if (creator == null)
							{
								throw new InvalidOperationException("Unable to calculate value for key " + key);
							}
							TValue value2 = creator(key);
							isStaleItem = false;
							value = new CacheItem<TValue>(value2);
							Contents[key] = value;
							RemoveOldItems_Locked();
						}
					}
				}
			}
			else
			{
				if (creator == null)
				{
					throw new InvalidOperationException("Unable to calculate value for key " + key);
				}
				value = new CacheItem<TValue>(creator(key));
				isStaleItem = false;
			}
			if (value == null)
			{
				throw new InvalidOperationException("Unable to find value for key " + key);
			}
			return value.Value;
		}

		private bool IsValidItem(CacheItem<TValue> item)
		{
			if (item == null)
			{
				return false;
			}
			DateTime t = DateTime.Now - MaximumItemLifespan;
			if (item.LastUseTime < t)
			{
				return false;
			}
			return true;
		}

		private void RemoveOldItems_Locked()
		{
			if (!(LastCacheClean + CacheClearPeriod > DateTime.Now))
			{
				DateTime t = DateTime.Now - MaximumItemLifespan;
				List<TKey> list = new List<TKey>();
				foreach (KeyValuePair<TKey, CacheItem<TValue>> content in Contents)
				{
					TKey key = content.Key;
					CacheItem<TValue> value = content.Value;
					if (value == null || value.LastUseTime < t)
					{
						list.Add(key);
					}
				}
				foreach (TKey item in list)
				{
					Contents.Remove(item);
				}
				LastCacheClean = DateTime.Now;
			}
		}
	}
}
