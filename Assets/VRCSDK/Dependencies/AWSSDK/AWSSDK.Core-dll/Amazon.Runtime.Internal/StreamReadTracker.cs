using Amazon.Util;
using System;

namespace Amazon.Runtime.Internal
{
	internal class StreamReadTracker
	{
		private object sender;

		private EventHandler<StreamTransferProgressArgs> callback;

		private long contentLength;

		private long totalBytesRead;

		private long totalIncrementTransferred;

		private long progressUpdateInterval;

		internal StreamReadTracker(object sender, EventHandler<StreamTransferProgressArgs> callback, long contentLength, long progressUpdateInterval)
		{
			this.sender = sender;
			this.callback = callback;
			this.contentLength = contentLength;
			this.progressUpdateInterval = progressUpdateInterval;
		}

		public void ReadProgress(int bytesRead)
		{
			if (callback != null && bytesRead > 0)
			{
				totalBytesRead += bytesRead;
				totalIncrementTransferred += bytesRead;
				if (totalIncrementTransferred >= progressUpdateInterval || totalBytesRead == contentLength)
				{
					AWSSDKUtils.InvokeInBackground(callback, new StreamTransferProgressArgs(totalIncrementTransferred, totalBytesRead, contentLength), sender);
					totalIncrementTransferred = 0L;
				}
			}
		}

		public void UpdateProgress(float progress)
		{
			int bytesRead = (int)((long)(progress * (float)contentLength) - totalBytesRead);
			ReadProgress(bytesRead);
		}
	}
}
