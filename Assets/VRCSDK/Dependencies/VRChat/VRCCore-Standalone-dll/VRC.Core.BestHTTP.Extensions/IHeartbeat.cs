using System;

namespace VRC.Core.BestHTTP.Extensions
{
	internal interface IHeartbeat
	{
		void OnHeartbeatUpdate(TimeSpan dif);
	}
}
