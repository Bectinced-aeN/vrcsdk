using Amazon.Runtime.Internal;
using System.Threading;
using UnityEngine;

namespace Amazon.Util.Storage.Internal
{
	public class NetworkInfo
	{
		public static NetworkReachability Reachability
		{
			get
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				if (UnityInitializer.IsMainThread())
				{
					return Application.get_internetReachability();
				}
				NetworkReachability _networkReachability = 0;
				AutoResetEvent asyncEvent = new AutoResetEvent(initialState: false);
				UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					_networkReachability = Application.get_internetReachability();
					asyncEvent.Set();
				});
				asyncEvent.WaitOne();
				return _networkReachability;
			}
		}
	}
}
