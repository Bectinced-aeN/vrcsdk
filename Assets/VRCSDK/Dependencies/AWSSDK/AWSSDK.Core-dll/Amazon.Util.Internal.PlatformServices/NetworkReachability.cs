using Amazon.Util.Storage.Internal;
using System;
using UnityEngine;

namespace Amazon.Util.Internal.PlatformServices
{
	public class NetworkReachability : INetworkReachability
	{
		internal EventHandler<NetworkStatusEventArgs> mNetworkReachabilityChanged;

		internal static readonly object reachabilityChangedLock = new object();

		public NetworkStatus NetworkStatus
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Invalid comparison between Unknown and I4
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Invalid comparison between Unknown and I4
				NetworkReachability reachability = NetworkInfo.Reachability;
				if ((int)reachability == 1)
				{
					return NetworkStatus.ReachableViaCarrierDataNetwork;
				}
				if ((int)reachability == 2)
				{
					return NetworkStatus.ReachableViaWiFiNetwork;
				}
				return NetworkStatus.NotReachable;
			}
		}

		public event EventHandler<NetworkStatusEventArgs> NetworkReachabilityChanged
		{
			add
			{
				lock (reachabilityChangedLock)
				{
					mNetworkReachabilityChanged = (EventHandler<NetworkStatusEventArgs>)Delegate.Combine(mNetworkReachabilityChanged, value);
				}
			}
			remove
			{
				lock (reachabilityChangedLock)
				{
					mNetworkReachabilityChanged = (EventHandler<NetworkStatusEventArgs>)Delegate.Remove(mNetworkReachabilityChanged, value);
				}
			}
		}

		internal void OnNetworkReachabilityChanged(NetworkStatus status)
		{
			mNetworkReachabilityChanged?.Invoke(null, new NetworkStatusEventArgs(status));
		}
	}
}
