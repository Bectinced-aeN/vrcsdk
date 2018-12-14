using VRC.Core.BestHTTP.SignalR.Hubs;

namespace VRC.Core.BestHTTP.SignalR.Messages
{
	internal struct ClientMessage
	{
		public readonly Hub Hub;

		public readonly string Method;

		public readonly object[] Args;

		public readonly ulong CallIdx;

		public readonly OnMethodResultDelegate ResultCallback;

		public readonly OnMethodFailedDelegate ResultErrorCallback;

		public readonly OnMethodProgressDelegate ProgressCallback;

		public ClientMessage(Hub hub, string method, object[] args, ulong callIdx, OnMethodResultDelegate resultCallback, OnMethodFailedDelegate resultErrorCallback, OnMethodProgressDelegate progressCallback)
		{
			Hub = hub;
			Method = method;
			Args = args;
			CallIdx = callIdx;
			ResultCallback = resultCallback;
			ResultErrorCallback = resultErrorCallback;
			ProgressCallback = progressCallback;
		}
	}
}
