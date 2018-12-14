using System;

namespace VRC.Core.BestHTTP.Caching
{
	internal sealed class HTTPCacheMaintananceParams
	{
		public TimeSpan DeleteOlder
		{
			get;
			private set;
		}

		public ulong MaxCacheSize
		{
			get;
			private set;
		}

		public HTTPCacheMaintananceParams(TimeSpan deleteOlder, ulong maxCacheSize)
		{
			DeleteOlder = deleteOlder;
			MaxCacheSize = maxCacheSize;
		}
	}
}
