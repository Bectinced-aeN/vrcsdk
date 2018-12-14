using System;

namespace Amazon.S3.Util
{
	public interface IAsyncCancelableResult : IAsyncResult
	{
		bool IsCanceled
		{
			get;
		}

		void Cancel();
	}
}
