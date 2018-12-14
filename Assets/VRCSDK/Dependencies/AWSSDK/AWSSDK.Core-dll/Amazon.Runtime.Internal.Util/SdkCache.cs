using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Amazon.Runtime.Internal.Util
{
	public static class SdkCache
	{
		internal class CacheKey
		{
			public ImmutableCredentials ImmutableCredentials
			{
				get;
				private set;
			}

			public RegionEndpoint RegionEndpoint
			{
				get;
				private set;
			}

			public string ServiceUrl
			{
				get;
				private set;
			}

			public object CacheType
			{
				get;
				private set;
			}

			private CacheKey()
			{
				ImmutableCredentials = null;
				RegionEndpoint = null;
				ServiceUrl = null;
				CacheType = null;
			}

			public static CacheKey Create(AmazonServiceClient client, object cacheType)
			{
				if (client == null)
				{
					throw new ArgumentNullException("client");
				}
				return new CacheKey
				{
					ImmutableCredentials = client.Credentials?.GetCredentials(),
					RegionEndpoint = client.Config.RegionEndpoint,
					ServiceUrl = client.Config.ServiceURL,
					CacheType = cacheType
				};
			}

			public static CacheKey Create(object cacheType)
			{
				return new CacheKey
				{
					CacheType = cacheType
				};
			}

			public override int GetHashCode()
			{
				return Hashing.Hash(ImmutableCredentials, RegionEndpoint, ServiceUrl, CacheType);
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				CacheKey cacheKey = obj as CacheKey;
				if (cacheKey == null)
				{
					return false;
				}
				return AWSSDKUtils.AreEqual(new object[4]
				{
					ImmutableCredentials,
					RegionEndpoint,
					ServiceUrl,
					CacheType
				}, new object[4]
				{
					cacheKey.ImmutableCredentials,
					cacheKey.RegionEndpoint,
					cacheKey.ServiceUrl,
					cacheKey.CacheType
				});
			}
		}

		private static object cacheLock = new object();

		private static Cache<CacheKey, ICache> cache = new Cache<CacheKey, ICache>();

		public static void Clear()
		{
			cache.Clear();
		}

		public static void Clear(object cacheType)
		{
			lock (cacheLock)
			{
				foreach (CacheKey key in cache.Keys)
				{
					if (AWSSDKUtils.AreEqual(key.CacheType, cacheType))
					{
						cache.GetValue(key, null).Clear();
					}
				}
			}
		}

		public static ICache<TKey, TValue> GetCache<TKey, TValue>(object client, object cacheIdentifier, IEqualityComparer<TKey> keyComparer)
		{
			return GetCache<TKey, TValue>(client as AmazonServiceClient, cacheIdentifier, keyComparer);
		}

		public static ICache<TKey, TValue> GetCache<TKey, TValue>(AmazonServiceClient client, object cacheIdentifier, IEqualityComparer<TKey> keyComparer)
		{
			CacheKey key = (client != null) ? CacheKey.Create(client, cacheIdentifier) : CacheKey.Create(cacheIdentifier);
			ICache cache = null;
			lock (cacheLock)
			{
				cache = SdkCache.cache.GetValue(key, (CacheKey k) => new Cache<TKey, TValue>(keyComparer));
			}
			Cache<TKey, TValue> cache2 = cache as Cache<TKey, TValue>;
			if (cache != null && cache2 == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to cast cache of type {0} as cache of type {1}", cache.GetType().FullName, typeof(Cache<TKey, TValue>).FullName));
			}
			return cache2;
		}
	}
}
