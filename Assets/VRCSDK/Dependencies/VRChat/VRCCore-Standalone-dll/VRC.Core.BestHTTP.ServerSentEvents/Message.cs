using System;

namespace VRC.Core.BestHTTP.ServerSentEvents
{
	internal sealed class Message
	{
		public string Id
		{
			get;
			internal set;
		}

		public string Event
		{
			get;
			internal set;
		}

		public string Data
		{
			get;
			internal set;
		}

		public TimeSpan Retry
		{
			get;
			internal set;
		}

		public override string ToString()
		{
			return $"\"{Event}\": \"{Data}\"";
		}
	}
}
