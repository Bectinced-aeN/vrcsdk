using System;

namespace Amazon.S3.Model
{
	public class TransferProgressArgs : EventArgs
	{
		private long _incrementTransferred;

		private long _total;

		private long _transferred;

		public int PercentDone => (int)(_transferred * 100 / _total);

		internal long IncrementTransferred => _incrementTransferred;

		public long TransferredBytes => _transferred;

		public long TotalBytes => _total;

		public TransferProgressArgs(long incrementTransferred, long transferred, long total)
		{
			_incrementTransferred = incrementTransferred;
			_transferred = transferred;
			_total = total;
		}

		public override string ToString()
		{
			return "Transfer Statistics. Percentage completed: " + PercentDone + ", Bytes transferred: " + _transferred + ", Total bytes to transfer: " + _total;
		}
	}
}
