using System;

namespace Amazon.Util.Internal.PlatformServices
{
	public class NetworkStatusEventArgs : EventArgs
	{
		public NetworkStatus Status
		{
			get;
			private set;
		}

		public NetworkStatusEventArgs(NetworkStatus status)
		{
			Status = status;
		}
	}
}
