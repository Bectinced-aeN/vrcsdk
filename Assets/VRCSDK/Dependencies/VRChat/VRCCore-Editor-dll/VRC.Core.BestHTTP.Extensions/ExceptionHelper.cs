using System;

namespace VRC.Core.BestHTTP.Extensions
{
	internal static class ExceptionHelper
	{
		public static Exception ServerClosedTCPStream()
		{
			return new Exception("TCP Stream closed unexpectedly by the remote server");
		}
	}
}
