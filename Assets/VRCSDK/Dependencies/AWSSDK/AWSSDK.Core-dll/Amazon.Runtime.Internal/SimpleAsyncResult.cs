using System;
using System.Threading;

namespace Amazon.Runtime.Internal
{
	internal class SimpleAsyncResult : IAsyncResult
	{
		public object AsyncState
		{
			get;
			private set;
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool CompletedSynchronously => true;

		public bool IsCompleted => true;

		public SimpleAsyncResult(object state)
		{
			AsyncState = state;
		}
	}
}
