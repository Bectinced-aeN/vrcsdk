using System;

namespace Amazon.Util.Internal.PlatformServices
{
	public interface INetworkReachability
	{
		NetworkStatus NetworkStatus
		{
			get;
		}

		event EventHandler<NetworkStatusEventArgs> NetworkReachabilityChanged;
	}
}
