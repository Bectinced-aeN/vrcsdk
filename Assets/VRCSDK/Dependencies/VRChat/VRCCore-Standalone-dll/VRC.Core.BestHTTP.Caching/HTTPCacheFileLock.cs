using System;
using System.Collections.Generic;

namespace VRC.Core.BestHTTP.Caching
{
	internal sealed class HTTPCacheFileLock
	{
		private static Dictionary<Uri, object> FileLocks = new Dictionary<Uri, object>();

		private static object SyncRoot = new object();

		internal static object Acquire(Uri uri)
		{
			lock (SyncRoot)
			{
				if (!FileLocks.TryGetValue(uri, out object value))
				{
					FileLocks.Add(uri, value = new object());
				}
				return value;
				IL_0037:
				object result;
				return result;
			}
		}

		internal static void Remove(Uri uri)
		{
			lock (SyncRoot)
			{
				if (FileLocks.ContainsKey(uri))
				{
					FileLocks.Remove(uri);
				}
			}
		}

		internal static void Clear()
		{
			lock (SyncRoot)
			{
				FileLocks.Clear();
			}
		}
	}
}
