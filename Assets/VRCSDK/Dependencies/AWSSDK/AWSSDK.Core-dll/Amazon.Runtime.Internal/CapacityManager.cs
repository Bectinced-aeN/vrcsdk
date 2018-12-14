using System;
using System.Collections.Generic;
using System.Threading;

namespace Amazon.Runtime.Internal
{
	public class CapacityManager : IDisposable
	{
		private bool _disposed;

		private static Dictionary<string, RetryCapacity> _serviceUrlToCapacityMap = new Dictionary<string, RetryCapacity>();

		private ReaderWriterLockSlim _rwlock = new ReaderWriterLockSlim();

		private readonly int THROTTLE_RETRY_REQUEST_COST;

		private readonly int THROTTLED_RETRIES;

		private readonly int THROTTLE_REQUEST_COST;

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				_rwlock.Dispose();
				_disposed = true;
			}
		}

		public CapacityManager(int throttleRetryCount, int throttleRetryCost, int throttleCost)
		{
			THROTTLE_RETRY_REQUEST_COST = throttleRetryCost;
			THROTTLED_RETRIES = throttleRetryCount;
			THROTTLE_REQUEST_COST = throttleCost;
		}

		public bool TryAcquireCapacity(RetryCapacity retryCapacity)
		{
			if (THROTTLE_RETRY_REQUEST_COST < 0)
			{
				return false;
			}
			lock (retryCapacity)
			{
				if (retryCapacity.AvailableCapacity - THROTTLE_RETRY_REQUEST_COST >= 0)
				{
					retryCapacity.AvailableCapacity -= THROTTLE_RETRY_REQUEST_COST;
					return true;
				}
				return false;
			}
		}

		public void TryReleaseCapacity(bool isRetryRequest, RetryCapacity retryCapacity)
		{
			if (isRetryRequest)
			{
				ReleaseCapacity(THROTTLE_RETRY_REQUEST_COST, retryCapacity);
			}
			else
			{
				ReleaseCapacity(THROTTLE_REQUEST_COST, retryCapacity);
			}
		}

		public RetryCapacity GetRetryCapacity(string serviceURL)
		{
			if (!TryGetRetryCapacity(serviceURL, out RetryCapacity value))
			{
				return AddNewRetryCapacity(serviceURL);
			}
			return value;
		}

		private bool TryGetRetryCapacity(string key, out RetryCapacity value)
		{
			_rwlock.EnterReadLock();
			try
			{
				if (_serviceUrlToCapacityMap.TryGetValue(key, out value))
				{
					return true;
				}
				return false;
			}
			finally
			{
				_rwlock.ExitReadLock();
			}
		}

		private RetryCapacity AddNewRetryCapacity(string serviceURL)
		{
			_rwlock.EnterUpgradeableReadLock();
			try
			{
				if (!_serviceUrlToCapacityMap.TryGetValue(serviceURL, out RetryCapacity value))
				{
					_rwlock.EnterWriteLock();
					try
					{
						value = new RetryCapacity(THROTTLE_RETRY_REQUEST_COST * THROTTLED_RETRIES);
						_serviceUrlToCapacityMap.Add(serviceURL, value);
						return value;
					}
					finally
					{
						_rwlock.ExitWriteLock();
					}
				}
				return value;
			}
			finally
			{
				_rwlock.ExitUpgradeableReadLock();
			}
		}

		private static void ReleaseCapacity(int capacity, RetryCapacity retryCapacity)
		{
			if (retryCapacity.AvailableCapacity >= 0 && retryCapacity.AvailableCapacity < retryCapacity.MaxCapacity)
			{
				lock (retryCapacity)
				{
					retryCapacity.AvailableCapacity = Math.Min(retryCapacity.AvailableCapacity + capacity, retryCapacity.MaxCapacity);
				}
			}
		}
	}
}
