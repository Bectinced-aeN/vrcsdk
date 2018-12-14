using System;
using System.Collections.Generic;

namespace Amazon.Runtime.Internal.Util
{
	public interface ICache
	{
		TimeSpan MaximumItemLifespan
		{
			get;
			set;
		}

		TimeSpan CacheClearPeriod
		{
			get;
			set;
		}

		int ItemCount
		{
			get;
		}

		void Clear();
	}
	public interface ICache<TKey, TValue> : ICache
	{
		List<TKey> Keys
		{
			get;
		}

		TValue GetValue(TKey key, Func<TKey, TValue> creator);

		TValue GetValue(TKey key, Func<TKey, TValue> creator, out bool isStaleItem);

		void Clear(TKey key);

		TOut UseCache<TOut>(TKey key, Func<TOut> operation, Action onError, Predicate<Exception> shouldRetryForException);
	}
}
